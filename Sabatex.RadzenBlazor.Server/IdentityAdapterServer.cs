using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sabatex.Core;
using Sabatex.Core.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sabatex.RadzenBlazor.Server;
/// <summary>
/// Provides server-side identity management functionality, including user registration, sign-in, sign-out, external
/// authentication, and user information updates. This class acts as an adapter for identity-related operations in a
/// server environment.
/// </summary>
/// <remarks>This class integrates with ASP.NET Core Identity components such as <see
/// cref="SignInManager{TUser}"/> and <see cref="UserManager{TUser}"/> to perform identity-related tasks. It also
/// provides methods for handling external authentication providers and managing user state during navigation.  The
/// class is designed to be used in server-side Blazor applications and assumes that the application is configured with
/// the necessary identity services.</remarks>
public class IdentityAdapterServer : IIdentityAdapter
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly ILogger<IdentityAdapterServer> _logger;
    private readonly NavigationManager _navigationManager;
    private readonly Core.Identity.IEmailSender<ApplicationUser> _emailSender;
    private readonly IStringLocalizer<IdentityAdapterServer> _localizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityAdapterServer"/> class, providing services for managing
    /// user authentication, user data, and related operations.
    /// </summary>
    /// <param name="signInManager">The <see cref="SignInManager{TUser}"/> used to handle user sign-in operations.</param>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/> used to manage user accounts, including creation, deletion, and updates.</param>
    /// <param name="contextAccessor">The <see cref="IHttpContextAccessor"/> used to access the current HTTP context.</param>
    /// <param name="userStore">The <see cref="IUserStore{TUser}"/> used to interact with the underlying user data store.</param>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> used for logging messages related to the <see
    /// cref="IdentityAdapterServer"/>.</param>
    /// <param name="navigationManager">The <see cref="NavigationManager"/> used to manage navigation and URI resolution.</param>
    /// <param name="emailSender">The <see cref="Core.IEmailSender{TUser}"/> used to send email messages to users.</param>
    /// <param name="stringLocalizer">The <see cref="IStringLocalizer{T}"/> used for localizing strings within the <see
    /// cref="IdentityAdapterServer"/>.</param>
    public IdentityAdapterServer(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 IHttpContextAccessor contextAccessor,
                                 IUserStore<ApplicationUser> userStore,
                                 ILogger<IdentityAdapterServer> logger,
                                 NavigationManager navigationManager,
                                 Core.Identity.IEmailSender<ApplicationUser> emailSender,
                                 IStringLocalizer<IdentityAdapterServer> stringLocalizer)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _userStore = userStore;
        _logger = logger;
        _navigationManager = navigationManager;
        _emailSender = emailSender;
        _localizer = stringLocalizer;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<ExternalProvider>> GetExternalProvidersAsync()
    {
        var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return schemes
            .Select(s => new ExternalProvider(s.Name, s.DisplayName));
    }
    /// <summary>
    /// Redirects the user to the specified URI.
    /// </summary>
    /// <remarks>If the provided URI is not a well-formed relative URI, it will be converted to a
    /// base-relative path. During static rendering, this method throws a <see cref="NavigationException"/> handled by
    /// the framework as a redirect.</remarks>
    /// <param name="uri">The target URI to redirect to. If <see langword="null"/> or empty, the redirection will default to the base
    /// relative path.</param>
    /// <exception cref="InvalidOperationException">Thrown if the method is called outside of a static rendering context.</exception>
    [DoesNotReturn]
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = _navigationManager.ToBaseRelativePath(uri);
        }

        // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
        // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
        _navigationManager.NavigateTo(uri);
        throw new InvalidOperationException($"{nameof(IdentityAdapterServer)} can only be used during static rendering.");
    }
    /// <summary>
    /// Redirects the user to the specified URI with the provided query parameters.
    /// </summary>
    /// <remarks>This method does not return to the caller, as it performs a redirection.</remarks>
    /// <param name="uri">The base URI to redirect to. This must be a valid relative or absolute URI.</param>
    /// <param name="queryParameters">A dictionary of query parameters to append to the URI. Keys represent parameter names, and values represent
    /// parameter values. Null values will be ignored.</param>
    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);

    }
    /// <summary>
    /// Redirects the client to the current page.
    /// </summary>
    /// <remarks>This method does not return to the caller and terminates the current execution
    /// context.</remarks>
    [DoesNotReturn]
    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);
    /// <summary>
    /// Redirects the user to the current page with the specified status cookie message.
    /// </summary>
    /// <param name="message">The status message to display on the redirected page. Cannot be null or empty.</param>
    [DoesNotReturn]
    public void RedirectToCurrentPageWithStatus(string message)=> RedirectToWithStatus(CurrentPath, message);
    /// <summary>
    /// Redirects the user to the specified URI and sets a status message in a cookie.
    /// </summary>
    /// <remarks>The status message is stored in a cookie using the name defined by <see
    /// cref="IIdentityAdapter.StatusCookieName"/>. The cookie is configured using the <see cref="StatusCookieBuilder"/>
    /// for the current HTTP context.</remarks>
    /// <param name="uri">The URI to which the user will be redirected. This cannot be null or empty.</param>
    /// <param name="message">The status message to store in a cookie. This cannot be null or empty.</param>
    public void RedirectToWithStatus(string uri, string message)
    {
        if (_contextAccessor.HttpContext is null)
        {
            throw new InvalidOperationException("HttpContext is not available. Ensure this method is called within a valid HTTP request context.");
        }
        _contextAccessor.HttpContext.Response.Cookies.Append((this as IIdentityAdapter).StatusCookieName, message, StatusCookieBuilder.Build(_contextAccessor.HttpContext));
        RedirectTo(uri);
    }
    /// <summary>
    /// Registers a new user with the specified email, password, and full name, and optionally redirects to a specified
    /// URL.
    /// </summary>
    /// <remarks>This method creates a new user account, sends an email confirmation link to the specified
    /// email address, and optionally signs the user in if account confirmation is not required. If the registration
    /// fails, an error message describing the failure is returned.</remarks>
    /// <param name="email">The email address of the user to register. This value cannot be null or empty.</param>
    /// <param name="password">The password for the new user. This value must meet the password policy requirements.</param>
    /// <param name="fullName">The full name of the user. If null or empty, the email address will be used as the user's name.</param>
    /// <param name="returnUrl">The URL to redirect to after successful registration. This value can be null.</param>
    /// <returns>A string containing an error message if the registration fails; otherwise, <see langword="null"/> if the
    /// registration is successful.</returns>
    public async Task<string?> RegisterUserAsync(string email, string password, string fullName,string returnUrl)
    {
        var user = CreateUser();
        user.FullName = fullName ?? email; // Use email as name if not provided

        await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
        var emailStore = GetEmailStore();
        await emailStore.SetEmailAsync(user, email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return result.Errors is null ? null : $"Error: {string.Join(", ", result.Errors.Select(error => error.Description))}";
        }

        _logger.LogInformation("User created a new account with password.");
        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = _navigationManager.GetUriWithQueryParameters(
            _navigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code, ["returnUrl"] = returnUrl });


        try
        {
            await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(callbackUrl));
        }
        catch (Exception e)
        {

        }

        if (_userManager.Options.SignIn.RequireConfirmedAccount)
        {
            RedirectTo(
                "Account/RegisterConfirmation",
                new  Dictionary<string, object?> { ["email"] = email, ["returnUrl"] = returnUrl });
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        RedirectTo(returnUrl);
        return null; // No error message, registration successful

    }
    /// <summary>
    /// Attempts to sign in a user using their email and password credentials.
    /// </summary>
    /// <remarks>This method clears any existing external authentication cookies before attempting to sign in
    /// the user. If the user is locked out, the method will return <see cref="SignInStatus.LockedOut"/> without
    /// attempting further authentication.</remarks>
    /// <param name="email">The email address of the user attempting to sign in. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="password">The password associated with the specified email. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="rememberMe">A value indicating whether the user's session should persist across browser restarts.</param>
    /// <returns>A <see cref="SignInStatus"/> value indicating the result of the sign-in attempt. Possible values include: <list
    /// type="bullet"> <item><description><see cref="SignInStatus.Success"/> if the sign-in was
    /// successful.</description></item> <item><description><see cref="SignInStatus.InvalidCredentials"/> if the email
    /// or password is incorrect.</description></item> <item><description><see cref="SignInStatus.LockedOut"/> if the
    /// user is locked out due to too many failed attempts.</description></item> <item><description><see
    /// cref="SignInStatus.RequiresTwoFactor"/> if two-factor authentication is required to complete the
    /// sign-in.</description></item> </list></returns>
    public async Task<SignInStatus> SignInAsync(string email, string password, bool rememberMe)
    {
        // Clear any existing external-cookie state so we start fresh
        await _contextAccessor.HttpContext!
            .SignOutAsync(IdentityConstants.ExternalScheme);


        // Lookup the user by email
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return SignInStatus.InvalidCredentials;

        // If the user is locked out, return LockedOut status
        if (await _userManager.IsLockedOutAsync(user))
            return SignInStatus.LockedOut;

        // Attempt the password sign-in
        var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded) return SignInStatus.Success;
        if (result.RequiresTwoFactor) return SignInStatus.RequiresTwoFactor;
        if (result.IsLockedOut) return SignInStatus.LockedOut;

        // Fallback to invalid credentials
        return SignInStatus.InvalidCredentials;
    }


    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }


    private string CurrentPath => _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);
    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5),
    };
    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
        }
    }
    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }

    public void RedirectTo(string uri, string? statusMessage = null)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ExternalProvider>> GetProvidersAsync()
    {
        throw new NotImplementedException();
    }

    public Task ChallengeAsync(string provider, string returnUrl)
    {
        throw new NotImplementedException();
    }

    public Task<ExternalLoginInfoDTO?> HandleCallbackAsync(string returnUrl, string? remoteError)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CompleteRegistrationAsync(ExtermnalRegisterDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            // new user
            user = CreateUser();
            user.FullName = model.Name ?? model.Email; // Use email as name if not provided
            var emailStore = GetEmailStore();
            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
            var r = await _userManager.CreateAsync(user);
            if (!r.Succeeded)
            {

                var message = $"Error: {string.Join(",", r.Errors.Select(error => error.Description))}";
                _navigationManager.NavigateTo(_navigationManager.GetUriWithQueryParameters("Account/Register", new Dictionary<string, object?> { ["error"] = message }), forceLoad: true);
                throw new Exception(message);
            }
            _logger.LogInformation("User created an account using {Name} provider.", model.Provider);
        }

        var result = await _userManager.AddLoginAsync(user, new UserLoginInfo(model.Provider, model.ProviderKey,""));
        if (result.Succeeded)
        {
            _logger.LogInformation("User add external account {Name} provider.", model.Provider);

            //var userId = await UserManager.GetUserIdAsync(user);
            //var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //var callbackUrl = NavigationManager.GetUriWithQueryParameters(
            //    NavigationManager.ToAbsoluteUri("Account/ConfirmEmail").AbsoluteUri,
            //    new Dictionary<string, object?> { ["userId"] = userId, ["code"] = code });
            //await EmailSender.SendConfirmationLinkAsync(user, Input.Email, HtmlEncoder.Default.Encode(callbackUrl));

            // If account confirmation is required, we need to show the link if we don't have a real email sender
            //if (UserManager.Options.SignIn.RequireConfirmedAccount)
            //{
            //    RedirectManager.RedirectTo("Account/RegisterConfirmation", new() { ["email"] = Input.Email });
            // }

            await _signInManager.SignInAsync(user, isPersistent: false, model.Provider);
            RedirectTo(model.ReturnUrl);
            return result.Succeeded;
        }
        return false; // Registration failed, return false  

    }

    public async Task<ApplicationUserDto> GetUserInfoAsync()
    {
        // This method should retrieve the user information from the database or context.
        var user =await  GetApplicationUserAsync();
        return new ApplicationUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

    }
    /// <summary>
    /// Updates the user information for the currently authenticated user.
    /// </summary>
    /// <remarks>This method updates the full name and phone number of the currently authenticated user.  If
    /// the provided user ID does not match the ID of the authenticated user, the update is aborted, and a warning is
    /// logged. Additionally, if any update operation fails (e.g., setting the phone number or updating the user's
    /// name),  the method logs the error and redirects to the current page with a status message.</remarks>
    /// <param name="userInfo">An <see cref="ApplicationUserDto"/> object containing the updated user information, including the user ID, full
    /// name, and phone number.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateUserInfoAsync(ApplicationUserDto userInfo)
    {
        var message = _localizer["Updating user info for user {0}", userInfo.Id];
        var user = await GetApplicationUserAsync();
        
        if (user.Id != userInfo.Id)
        {
            message = _localizer["Attempt to update user info for a different user. User ID: {0}, Attempted ID: {1}", user.Id, userInfo.Id];
            _logger.LogWarning(message);
            RedirectToCurrentPageWithStatus(message);
            return; // User ID mismatch
        }
        var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, userInfo.PhoneNumber);
        if (!setPhoneResult.Succeeded)
        {
            message = _localizer["Failed to set phone number for user {0}. Error: {1}", userInfo.Id, string.Join(", ", setPhoneResult.Errors.Select(e => e.Description))];
            _logger.LogWarning(message);
            RedirectToCurrentPageWithStatus(message);
            return; // Failed to set phone number
        }
        
        user.FullName = userInfo.FullName;
        var setNameResult = await _userManager.UpdateAsync(user);
        if (!setNameResult.Succeeded)
        {
            message = _localizer["Failed to update user name for user {0}. Error: {1}", userInfo.Id, string.Join(", ", setNameResult.Errors.Select(e => e.Description))];
            _logger.LogWarning(message);
            RedirectToCurrentPageWithStatus(message);
            return; // Failed to update user name
        }
        await _signInManager.RefreshSignInAsync(user);
        RedirectTo("/");
        return; // User info updated successfully

    }
    /// <summary>
    /// Retrieves the status message stored in the HTTP request cookies.
    /// </summary>
    /// <remarks>This method accesses the HTTP context to retrieve the value of the cookie identified by the 
    /// <see cref="IIdentityAdapter.StatusCookieName"/> property. Ensure that the HTTP context and the  required cookie
    /// are available when calling this method.</remarks>
    /// <returns>A <see cref="string"/> containing the status message from the cookie, or <see langword="null"/> if the cookie is
    /// not found.</returns>
    public async Task<string?> GetStatusMessage()
    {
        await Task.Yield(); // Ensure we are not blocking the thread
        return _httpContext.Request.Cookies[(this as IIdentityAdapter).StatusCookieName];
    }

    HttpContext _httpContext => _contextAccessor.HttpContext ?? throw new InvalidOperationException(_localizer["HttpContext is not available. Ensure this method is called within a valid HTTP request context."]);

    async Task<ApplicationUser> GetApplicationUserAsync()
    { 
        var user = await _userManager.GetUserAsync(_httpContext.User);
        if (user == null)
        {
            throw new InvalidOperationException(_localizer["User is not authenticated. Ensure this method is called within a valid user context."]);
        }
        return user;
    }
}
