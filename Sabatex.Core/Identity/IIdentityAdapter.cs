using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Sabatex.Core.LocalizerHelper; 

namespace Sabatex.Core.Identity;
public record ExternalProvider(string Name, string DisplayName);

public interface IIdentityAdapter
{
    /// <summary>
    /// Gets the name of the cookie used to store status messages related to identity operations.
    /// </summary>
    public string StatusCookieName => "Identity.StatusMessage";
    /// <summary>
    /// Read all roles in application
    /// </summary>
    /// <returns></returns>
    //IEnumerable<string> GetAvailableRoles();
    Task<SignInStatus> SignInAsync(string email, string password, bool rememberMe);
    /// <summary>
    /// Returns a list of configured external login providers.
    /// </summary>
    Task<IEnumerable<ExternalProvider>> GetExternalProvidersAsync();

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
 

    /// <summary>Повернути перелік доступних зовнішніх провайдерів</summary>
    Task<IEnumerable<ExternalProvider>> GetProvidersAsync();

    /// <summary>Започаткувати Challenge/302 до провайдера</summary>
    Task ChallengeAsync(string provider, string returnUrl);

    /// <summary>Опрацювати callback від провайдера, повернути ExternalLoginInfoDto або null</summary>
    Task<ExternalLoginInfoDTO?> HandleCallbackAsync(string returnUrl, string? remoteError);

    /// <summary>Провести реєстрацію (або прив’язку) зовнішнього логіну</summary>
    Task<bool> CompleteRegistrationAsync(ExtermnalRegisterDTO model);


}
