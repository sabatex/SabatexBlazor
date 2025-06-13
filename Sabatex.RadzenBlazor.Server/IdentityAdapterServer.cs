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

public class IdentityAdapterServer : IIdentityAdapter
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly ILogger<IdentityAdapterServer> _logger;
    private readonly NavigationManager _navigationManager;
    private readonly Core.IEmailSender<ApplicationUser> _emailSender;
    private readonly IStringLocalizer<IdentityAdapterServer> _localizer;


    public IdentityAdapterServer(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 IHttpContextAccessor contextAccessor,
                                 IUserStore<ApplicationUser> userStore,
                                 ILogger<IdentityAdapterServer> logger,
                                 NavigationManager navigationManager,
                                 Core.IEmailSender<ApplicationUser> emailSender,
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

    public async Task<IEnumerable<ExternalProvider>> GetExternalProvidersAsync()
    {
        var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return schemes
            .Select(s => new ExternalProvider(s.Name, s.DisplayName));
    }

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
    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);

    }
    [DoesNotReturn]
    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

    [DoesNotReturn]
    public void RedirectToCurrentPageWithStatus(string message)=> RedirectToWithStatus(CurrentPath, message);
    
    public void RedirectToWithStatus(string uri, string message)
    {
        _contextAccessor.HttpContext.Response.Cookies.Append((this as IIdentityAdapter).StatusCookieName, message, StatusCookieBuilder.Build(_contextAccessor.HttpContext));
        RedirectTo(uri);
    }

    public async Task<string?> RegisterUserAsync(string email, string password, string fullName,string returnUrl)
    {
        var user = CreateUser();
        user.Name = fullName ?? email; // Use email as name if not provided

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
            user.Name = model.Name ?? model.Email; // Use email as name if not provided
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
        var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
        return new ApplicationUserDto
        {
            Id = user.Id,
            FullName = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

    }

    public async Task UpdateUserInfoAsync(ApplicationUserDto userInfo)
    {
        var message = _localizer["Updating user info for user {0}", userInfo.Id];
        var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
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
        
        user.Name = userInfo.FullName;
        var setNameResult = await _userManager.UpdateAsync(user);
        if (!setNameResult.Succeeded)
        {
            message = _localizer["Failed to update user name for user {0}. Error: {1}", userInfo.Id, string.Join(", ", setNameResult.Errors.Select(e => e.Description))];
            _logger.LogWarning(message);
            RedirectToCurrentPageWithStatus(message);
            return; // Failed to update user name
        }
        RedirectToCurrentPageWithStatus(message);
        return; // User info updated successfully

    }

    public async Task<string> GetStatusMessage()
    {
        await Task.Yield(); // Ensure we are not blocking the thread

        return _contextAccessor.HttpContext.Request.Cookies[(this as IIdentityAdapter).StatusCookieName];
    }
}
