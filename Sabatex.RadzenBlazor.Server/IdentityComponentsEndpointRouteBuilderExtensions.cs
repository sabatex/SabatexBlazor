using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sabatex.Core.Identity;
using Sabatex.RadzenBlazor.Server;
using System.Security.Claims;
using System.Text.Json;


namespace Microsoft.AspNetCore.Routing;
/// <summary>
/// Provides extension methods for configuring endpoint routes required by Identity Razor components, including
/// authentication, account management, and personal data endpoints.
/// </summary>
/// <remarks>This class contains methods that register endpoints necessary for Identity-related functionality in
/// applications using Razor components. The endpoints support external login flows, user account management, and
/// personal data operations. These extensions are intended to be used during application startup to ensure all required
/// Identity endpoints are available for the Identity UI components.</remarks>
public static class IdentityComponentsEndpointRouteBuilderExtensions
{
    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };
    private static bool IsLocalUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;
        // must start with single '/' and must not be protocol absolute
        if (url.StartsWith('/') && !url.StartsWith("//") && !url.Contains("://"))
            return true;
        return false;
    }
    private static string LocalSafe(HttpContext ctx, string? url)
    {
        if (IsLocalUrl(url))
            return url!;
        return ctx.Request.PathBase.HasValue ? ctx.Request.PathBase.Value! : "/";
    }


    /// <summary>
    /// Maps additional identity-related endpoints required for account management and external authentication to the
    /// specified endpoint route builder.
    /// </summary>
    /// <remarks>This method registers endpoints necessary for external login, logout, personal data download,
    /// and role management, supporting integration with Identity Razor components and external authentication
    /// providers. The mapped endpoints include routes for performing external login, handling external login callbacks,
    /// logging out, linking external logins, downloading personal data, and retrieving available roles. All endpoints
    /// are grouped under the '/Account' route, with management endpoints requiring authorization.</remarks>
    /// <param name="endpoints">The endpoint route builder to which the identity endpoints will be mapped. Cannot be null.</param>
    /// <returns>An endpoint convention builder that can be used to further customize the mapped identity endpoints.</returns>
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
            // Build callback URI with lower-case returnUrl to match server expectation
            var query = new[]
            {
                new KeyValuePair<string,string>("returnUrl", returnUrl),
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
            return Results.Challenge(props, new[] { provider });
        });
        
        // 1.2) GET-ендпоінт, на який провайдер редіректить із кодом та state
        accountGroup.MapGet("/ExternalLogin",
            async (HttpContext context,
                           [FromServices] SignInManager<ApplicationUser> signInManager,
                           [FromServices]UserManager<ApplicationUser> userManager,
                           [FromServices] IMemoryCache cache,
                           [FromServices] ILogger<IdentityAdapterServer> logger,
                           [FromQuery] string? returnUrl,
                           [FromQuery] string? remoteError) =>
            {
                // 1) Якщо провайдер повернув помилку — одразу назад на /Account/Login
                if (!string.IsNullOrEmpty(remoteError))
                {
                    var msg = $"External provider returned error: {remoteError}";
                    logger.LogWarning(msg);
                    context.Response.Cookies.Append("Identity.StatusMessage", msg, StatusCookieBuilder.Build(context));
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                // 2) Завантажуємо info з external-cookie, який Middleware створив
                var info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    var msg = "ExternalLoginInfo is null (external cookie missing).";
                    logger.LogWarning(msg);
                    context.Response.Cookies.Append("Identity.StatusMessage", msg, StatusCookieBuilder.Build(context));
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                logger.LogDebug("External login info received from provider {Provider}", info.LoginProvider);
                // 3) Прагнемо залогінити існуючого користувача
                var signInResult = await signInManager.ExternalLoginSignInAsync(
                        info.LoginProvider,
                        info.ProviderKey,
                        isPersistent: false,
                        bypassTwoFactor: true);

                if (signInResult.Succeeded)
                {
                    // якщо є локальний профіль → повертаємося куди треба
                    logger.LogInformation("External login succeeded for provider {Provider}", info.LoginProvider);
                    context.Response.Redirect(LocalSafe(context, returnUrl));
                    return;
                }

                if (signInResult.IsLockedOut)
                {
                    // якщо користувач заблокований → редіректимо на сторінку блокування
                    logger.LogWarning("User locked out during external login.");
                    context.Response.Redirect("/Account/Lockout");
                    return;
                }
                if (signInResult.RequiresTwoFactor)
                {
                    logger.LogInformation("External login requires two-factor authentication.");
                    var twoFaUrl = QueryHelpers.AddQueryString("/Account/Login2FA", new Dictionary<string, string?>
                    {
                        ["returnUrl"] = LocalSafe(context, returnUrl)
                    });
                    context.Response.Redirect(twoFaUrl);
                    return;
                }

                if (signInResult.IsNotAllowed)
                {
                    logger.LogWarning("External login is not allowed (IsNotAllowed) for provider {Provider}.", info.LoginProvider);
                    context.Response.Cookies.Append("Identity.StatusMessage", "External login is not allowed for this account.", StatusCookieBuilder.Build(context));
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                // 4) Нова реєстрація → переходимо в Blazor-сторінку реєстрації
                //    Передаємо мінімальний набір даних через query-рядок
                var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? "";
                var name = info.Principal.FindFirstValue(ClaimTypes.Name)
                        ?? info.Principal.FindFirstValue(ClaimTypes.GivenName)
                        ?? "";

                // Check if a user with this email already exists
                ApplicationUser? existingUser = null;
                if (!string.IsNullOrEmpty(email))
                {
                    existingUser = await userManager.FindByEmailAsync(email);
                }

                if (existingUser != null)
                {
                    // Try to attach external login to existing user (if not already linked) and sign in
                    var addLoginResult = await userManager.AddLoginAsync(existingUser, info);
                    if (addLoginResult.Succeeded)
                    {
                        logger.LogInformation("Linked external login {Provider} to existing user {Email}", info.LoginProvider, email);
                        await signInManager.SignInAsync(existingUser, isPersistent: false);
                        context.Response.Redirect(LocalSafe(context, returnUrl));
                        return;
                    }
                    logger.LogWarning("Failed to link external login to existing user {Email}: {Errors}", email,
                        string.Join(';', addLoginResult.Errors.Select(e => e.Code)));
                    // Fall through to registration UI as a fallback
                }


                // Store ExternalLoginInfo server-side (IMemoryCache) under a short-lived nonce to avoid leaking providerKey
                var nonce = Guid.NewGuid().ToString("N");
                var cacheKey = $"ExternalLoginInfo:{nonce}";
                var cacheOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
                cache.Set(cacheKey, info, cacheOptions);

                var qs = new Dictionary<string, string?>
                {
                    ["returnUrl"] = LocalSafe(context, returnUrl),
                    ["provider"] = info.LoginProvider,
                    ["nonce"] = nonce,
                    ["email"] = email,
                    ["name"] = name,
                    ["existingUser"] = (existingUser != null).ToString().ToLowerInvariant()
                };

                var registerUrl = QueryHelpers.AddQueryString("/Account/ExternalLoginRegister", qs);
                logger.LogInformation("Redirecting to ExternalLoginRegister for provider {Provider} (nonce={Nonce}).", info.LoginProvider, nonce);
                context.Response.Redirect(registerUrl);
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


        manageGroup.MapGet("/roles",async ([FromServices] IIdentityAdapter adapter) => await adapter.GetAvailableRolesAsync());

        var apiGroup = endpoints.MapGroup("/api");
        apiGroup.MapGet($"/{nameof(ApplicationUserDto)}", async ([FromServices] IIdentityAdapter adapter) => await adapter.GetAvailableRolesAsync());


        return accountGroup;
    }
}
