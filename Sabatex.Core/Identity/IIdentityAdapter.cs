using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Sabatex.Core.LocalizerHelper; 

namespace Sabatex.Core.Identity;
/// <summary>
/// Represents an external authentication provider, such as a third-party identity provider.
/// </summary>
/// <param name="Name">The unique identifier of provider (Microsoft,Google,...).</param>
/// <param name="DisplayName">The user-friendly name of the external provider, typically displayed in the UI.</param>
public record ExternalProvider(string Name, string? DisplayName);
/// <summary>
/// Defines an abstraction for identity-related operations, including user authentication,  external login management,
/// user registration, and redirection handling.
/// </summary>
/// <remarks>This interface provides methods for common identity operations such as signing in,  registering
/// users, managing external login providers, and handling redirections.  It is designed to be implemented by classes
/// that integrate with identity systems  in web applications.</remarks>
public interface IIdentityAdapter
{
    /// <summary>
    /// Gets the name of the cookie used to store status messages related to identity operations.
    /// </summary>
    public string StatusCookieName => "Identity.StatusMessage";
    //IEnumerable<string> GetAvailableRoles();
    
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

    Task<SignInStatus> SignInAsync(string email, string password, bool rememberMe);
    /// <summary>
    /// Returns a list of configured external login providers.
    /// </summary>
    Task<IEnumerable<ExternalProvider>> GetExternalProvidersAsync();
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
    Task<string?> RegisterUserAsync(string email, string password,string fullName, string returnUrl);

    Task SignOutAsync();

    void RedirectTo(string? uri);
    void RedirectTo(string uri, Dictionary<string, object?> parameters);

     /// <summary>
     /// Редірект на uri з опціональним статус-повідомленням (без HttpContext!)
     /// </summary>
    void RedirectTo(string uri, string? statusMessage = null);


    void RedirectToCurrentPage();
    void RedirectToWithStatus(string uri, string message);
    void RedirectToCurrentPageWithStatus(string message);
 

    /// <summary>
    /// Asynchronously retrieves a collection of external authentication providers.
    /// </summary>
    /// <remarks>This method is typically used to obtain a list of external providers for authentication
    /// purposes,  such as social login providers (e.g., Google, Facebook). The caller can use the returned collection 
    /// to display available options to the user.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of
    /// ExternalProvider objects representing  the available external authentication providers. The collection will be
    /// empty if no providers are available.</returns>
    Task<IEnumerable<ExternalProvider>> GetProvidersAsync();

    /// <summary>Започаткувати Challenge/302 до провайдера</summary>
    Task ChallengeAsync(string provider, string returnUrl);

    /// <summary>Опрацювати callback від провайдера, повернути ExternalLoginInfoDto або null</summary>
    Task<ExternalLoginInfoDTO?> HandleCallbackAsync(string returnUrl, string? remoteError);

    /// <summary>Провести реєстрацію (або прив’язку) зовнішнього логіну</summary>
    Task<bool> CompleteRegistrationAsync(ExtermnalRegisterDTO model);
    /// <summary>
    /// Asynchronously retrieves information about the currently authenticated user.
    /// </summary>
    /// <remarks>This method requires the user to be authenticated. If no user is authenticated, the result
    /// may be null or an exception may be thrown, depending on the implementation.</remarks>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.  The task result contains an <see
    /// cref="ApplicationUserDto"/> object with details about the authenticated user.</returns>
    Task<ApplicationUserDto> GetUserInfoAsync();

    Task UpdateUserInfoAsync(ApplicationUserDto userInfo);
    /// <summary>
    /// Retrieves the status message stored in the HTTP request cookies.
    /// </summary>
    /// <remarks>This method accesses the HTTP context to retrieve the value of the cookie identified by the 
    /// <see cref="IIdentityAdapter.StatusCookieName"/> property. Ensure that the HTTP context and the  required cookie
    /// are available when calling this method.</remarks>
    /// <returns>A <see cref="string"/> containing the status message from the cookie, or <see langword="null"/> if the cookie is
    /// not found.</returns>

    Task<string?> GetStatusMessage();

    /// <summary>
    /// Asynchronously retrieves a collection of available roles.
    /// </summary>
    /// <remarks>The returned collection may be empty if no roles are available. The roles are typically used 
    /// for authorization or access control purposes.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of strings, where
    /// each string represents the name of an available role.</returns>
    Task<IEnumerable<string>> GetAvailableRolesAsync();
    /// <summary>
    /// Retrieves detailed information about a user based on their unique identifier.
    /// </summary>
    /// <param name="Id">The unique identifier of the user whose information is to be retrieved. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApplicationUserDto"/>
    /// object  with the user's information, or <see langword="null"/> if no user is found with the specified
    /// identifier.</returns>
    Task<ApplicationUserDto> GetUserInfoAsync(string Id);

}
