using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Sabatex.Core.Identity;
using Sabatex.RadzenBlazor.Server;
using System.Security.Claims;
using System.Text.Json;


namespace Microsoft.AspNetCore.Routing
{
    public static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        private static readonly CookieBuilder StatusCookieBuilder = new()
        {
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true,
            MaxAge = TimeSpan.FromSeconds(5),
        };


        // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            var accountGroup = endpoints.MapGroup("/Account");

            accountGroup.MapPost("/PerformExternalLogin", (
                HttpContext context,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromForm] string provider,
                [FromForm] string returnUrl) =>
            {
                // 1.1) query для callback-Uri
                var query = new[]
                {
                    new KeyValuePair<string,string>("ReturnUrl", returnUrl),
                    new KeyValuePair<string,string>("Action", IdentityExtensions.LoginCallbackAction)
                };

                // 1.2) новий callback шлях – прив’язаний до Blazor-сторінки реєстрації
                var callbackPath = "/Account/ExternalLogin";
                var callbackUri = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    callbackPath,
                    QueryString.Create(query));

                // 1.3) генеруємо властивості та повертаємо 302 Challenge
                var props = signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUri);
                return TypedResults.Challenge(props, [provider]);

             });
            
            // 1.2) GET-ендпоінт, на який провайдер редіректить із кодом та state
            accountGroup.MapGet("/ExternalLogin",
                async (HttpContext context,
                               [FromServices] SignInManager<ApplicationUser> signInManager,
                               [FromServices]UserManager<ApplicationUser> userManager,
                               [FromServices] NavigationManager _navigationManager,
                               [FromQuery] string? returnUrl,
                               [FromQuery] string? remoteError) =>
                {
                    // 1) Якщо провайдер повернув помилку — одразу назад на /Account/Login
                    if (!string.IsNullOrEmpty(remoteError))
                    {
                        var msg = $"Error from external provider: {remoteError}";
                        context.Response.Cookies.Append("Identity.StatusMessage", msg, StatusCookieBuilder.Build(context));
                        context.Response.Redirect("/Account/Login");
                        return;
                    }

                    // 2) Завантажуємо info з external-cookie, який Middleware створив
                    var info = await signInManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        context.Response.Cookies.Append("Identity.StatusMessage", "Error loading external login information.", StatusCookieBuilder.Build(context));
                        context.Response.Redirect("/Account/Login");
                        return;
                    }

                    // 3) Прагнемо залогінити існуючого користувача
                    var result = await signInManager.ExternalLoginSignInAsync(
                            info.LoginProvider,
                            info.ProviderKey,
                            isPersistent: false,
                            bypassTwoFactor: true);

                    if (result.Succeeded)
                    {
                        // якщо є локальний профіль → повертаємося куди треба
                        var basePath = context.Request.PathBase.HasValue ? context.Request.PathBase.Value : "/";
                        context.Response.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? basePath : $"{basePath}{returnUrl}");
                        return;
                    }

                    if (result.IsLockedOut)
                    {
                        // якщо користувач заблокований → редіректимо на сторінку блокування
                        context.Response.Redirect("/Account/Lockout");
                        return;
                    }

                    // 4) Нова реєстрація → переходимо в Blazor-сторінку реєстрації
                    //    Передаємо мінімальний набір даних через query-рядок
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
                    var name = info.Principal.FindFirstValue(ClaimTypes.Name)
                            ?? info.Principal.FindFirstValue(ClaimTypes.GivenName)
                            ?? "";
                    // Перевіряємо, чи є в системі користувач з цим email
                    var existingUser = !string.IsNullOrEmpty(email)
                        && (await userManager.FindByEmailAsync(email) != null);


                    var qs = new Dictionary<string, string?>
                    {
                        ["returnUrl"] = returnUrl,
                        ["provider"] = info.LoginProvider,
                        ["providerKey"] = info.ProviderKey,
                        ["email"] = email,
                        ["name"] = name,
                        ["existingUser"] = existingUser.ToString().ToLowerInvariant()

                    };
                    var url = QueryHelpers.AddQueryString("/Account/ExternalLoginRegister", qs);
                    
                    context.Response.Redirect(url);

                });


            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user,
                SignInManager<ApplicationUser> signInManager,
                [FromForm] string returnUrl) =>
            {
                await signInManager.SignOutAsync();
               //var returnUrl = "";
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });

            var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

            manageGroup.MapPost("/LinkExternalLogin", async (
                HttpContext context,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromForm] string provider) =>
            {
                // Clear the existing external cookie to ensure a clean login process
                await context.SignOutAsync(IdentityConstants.ExternalScheme);

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/Manage/ExternalLogins",
                    QueryString.Create("Action", IdentityExtensions.LinkLoginCallbackAction));

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
                return TypedResults.Challenge(properties, [provider]);
            });

            var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

            manageGroup.MapPost("/DownloadPersonalData", async (
                HttpContext context,
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                {
                    return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                }

                var userId = await userManager.GetUserIdAsync(user);
                downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

                // Only include personal data for download
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await userManager.GetLoginsAsync(user);
                foreach (var l in logins)
                {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
                var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
                return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
            });




            return accountGroup;
        }
    }
}
