using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Sabatex.Core.Identity;
using Sabatex.RadzenBlazor.Models;
using System.Security.Claims;

namespace Sabatex.RadzenBlazor;

/// <summary>
/// Provides an <see cref="AuthenticationStateProvider"/> that determines the user's authentication state based on data
/// persisted in the page during server-side rendering. The authentication state remains fixed for the lifetime of the
/// WebAssembly application.
/// </summary>
/// <remarks>This provider is intended for use in Blazor WebAssembly applications where authentication information
/// is established at the time of page rendering and does not change until a full page reload occurs. It supplies user
/// name and email for display purposes only and does not include authentication tokens for server requests.
/// Authentication for server communication is handled separately via cookies included in <see cref="HttpClient"/>
/// requests.</remarks>
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    IEnumerable<Claim> GetClaims(UserInfo userInfo)
    {
            yield return new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString());
            yield return new Claim(CustomClaimTypes.FullName, userInfo.Name);
            yield return new Claim(ClaimTypes.Email, userInfo.Email);
            foreach (var role in userInfo.Roles)
            {
                yield return new Claim(ClaimTypes.Role, role);
            }

    }

    /// <summary>
    /// Initializes a new instance of the PersistentAuthenticationStateProvider class using the specified persistent
    /// component state.
    /// </summary>
    /// <remarks>If the persistent state does not contain valid user information, the authentication state
    /// will not be initialized. This constructor is typically used to restore authentication state across page reloads
    /// or navigation in Blazor applications.</remarks>
    /// <param name="state">The persistent component state containing serialized user information used to establish the authentication
    /// state. Cannot be null.</param>
    public PersistentAuthenticationStateProvider(PersistentComponentState state)
    {
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
        {
            return;
        }
        
        IEnumerable<Claim>  claims = GetClaims(userInfo);

        authenticationStateTask = Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims,
                authenticationType: nameof(PersistentAuthenticationStateProvider)))));
    }
    /// <summary>
    /// Asynchronously retrieves the current authentication state for the user.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="AuthenticationState"/>
    /// object describing the user's authentication status.</returns>
    public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
}
