using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sabatex.Core.Identity;
using System.Diagnostics;

namespace Sabatex.RadzenBlazor.Server;

// This is a server-side AuthenticationStateProvider that uses PersistentComponentState to flow the
// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
/// <summary>
/// Provides a server-side authentication state provider that persists authentication state using
/// PersistentComponentState, enabling the authentication state to be transferred to the client and remain fixed for the
/// lifetime of the WebAssembly application.
/// </summary>
/// <remarks>This provider is intended for scenarios where authentication state must be established on the server
/// and reliably flowed to the client, such as during prerendering with Blazor WebAssembly. The persisted authentication
/// state is available to the client application and does not change after initial transfer. This class should be
/// disposed when no longer needed to release resources associated with state persistence.</remarks>
public sealed class PersistingServerAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private readonly PersistentComponentState state;
    private readonly IdentityOptions options;

    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState>? authenticationStateTask;
    /// <summary>
    /// Initializes a new instance of the PersistingServerAuthenticationStateProvider class using the specified
    /// persistent component state and identity options.
    /// </summary>
    /// <remarks>This constructor sets up authentication state persistence for interactive WebAssembly
    /// scenarios. The provided PersistentComponentState is used to register a callback for persisting authentication
    /// state when required.</remarks>
    /// <param name="persistentComponentState">The PersistentComponentState instance used to persist authentication state across server and client
    /// interactions.</param>
    /// <param name="optionsAccessor">An IOptions<IdentityOptions> instance that provides configuration settings for identity management.</param>
    public PersistingServerAuthenticationStateProvider(
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
    {
        state = persistentComponentState;
        options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(options.ClaimsIdentity.UserIdClaimType)?.Value;
            var email = principal.FindFirst(options.ClaimsIdentity.EmailClaimType)?.Value;
            var roles = principal.FindAll(options.ClaimsIdentity.RoleClaimType).Select(s=>s.Value).ToArray();
            var name = principal.FindFirst(CustomClaimTypes.FullName)?.Value ?? "";


            if (userId != null && email != null)
            {
                state.PersistAsJson("UserInfo", new 
                {
                    Id = userId,
                    Email = email,
                    Name = name,
                    Roles = roles 
                });
            }
        }
    }
    /// <summary>
    /// Releases all resources used by the instance and unsubscribes from authentication state change notifications.
    /// </summary>
    /// <remarks>Call this method when the instance is no longer needed to ensure that event handlers are
    /// detached and resources are properly released. After calling <see cref="Dispose"/>, the instance should not be
    /// used.</remarks>
    public void Dispose()
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }
}
