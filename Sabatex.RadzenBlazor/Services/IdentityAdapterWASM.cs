using Microsoft.AspNetCore.Components;
using Sabatex.Core.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Services;

public class IdentityAdapterWasm : IIdentityAdapter
{
    private readonly HttpClient _http;
    private readonly NavigationManager _navigationManager;

    public IdentityAdapterWasm(HttpClient http, NavigationManager navigationManager)
    {
        _http = http;
        _navigationManager = navigationManager;
    }

    public virtual async Task<IEnumerable<ExternalProvider>> GetExternalProvidersAsync()
    {
        try
        {
            // your API endpoint returns ExternalProvider[]
            return await _http
                       .GetFromJsonAsync<ExternalProvider[]>("api/account/externallogins")
                   ?? Array.Empty<ExternalProvider>();
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Error fetching external providers: {ex.Message}");
            return Array.Empty<ExternalProvider>();
        }
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
        throw new InvalidOperationException($"{nameof(IdentityAdapterWasm)} can only be used during static rendering.");

    }
    [DoesNotReturn]
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }
    private string CurrentPath => _navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path);

    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

    public void RedirectToCurrentPageWithStatus(string message) => RedirectToWithStatus(CurrentPath, message);
  
    public void RedirectToWithStatus(string uri, string message)
    {
        var t =  _http.PostAsJsonAsync("/api/account/redirecTo", new { uri, message });
        t.Wait(); // Wait for the task to complete, or handle it asynchronously as needed.
        throw new NotImplementedException("RedirectToWithStatus is not implemented in IdentityAdapterWasm. You can implement it to handle redirects with status messages.");
    }

    public async Task<string> RegisterUserAsync(string email, string password, string fullName, string returnUrl)
    {
        var response = await _http.PostAsJsonAsync("/api/account/register", new { email, password, fullName, returnUrl });
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<SignInStatus> SignInAsync(string email, string password, bool rememberMe)
    {
        var response = await _http.PostAsJsonAsync("/api/account/login", new { email, password, rememberMe });
        if  (response.IsSuccessStatusCode)
        {
            return response.Content.ReadFromJsonAsync<SignInStatus>().Result;
        }
        else
        {
            //   var error = await response.Content.ReadAsStringAsync();
            return SignInStatus.InvalidCredentials;
        }
    }

    public async Task SignOutAsync()
    {
        await _http.PostAsync("/api/account/logout", null);
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

    public Task<bool> CompleteRegistrationAsync(ExtermnalRegisterDTO model)
    {
        throw new NotImplementedException();
    }

    public Task<ApplicationUserDto> GetUserInfoAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUserInfoAsync(ApplicationUserDto userInfo)
    {
        throw new NotImplementedException();
    }

    Task IIdentityAdapter.UpdateUserInfoAsync(ApplicationUserDto userInfo)
    {
        return UpdateUserInfoAsync(userInfo);
    }

    public Task<string> GetStatusMessage()
    {
        throw new NotImplementedException();
    }
}