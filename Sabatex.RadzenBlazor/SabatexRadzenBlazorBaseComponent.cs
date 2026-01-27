using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sabatex.RadzenBlazor
{
    /// <summary>
    /// Provides a base class for Radzen Blazor components with common services and utility methods for HTTP
    /// communication, navigation, notifications, JavaScript interop, authentication, tooltips, and component state
    /// management.
    /// </summary>
    /// <remarks>This abstract class is intended to be inherited by custom Radzen Blazor components to
    /// simplify access to frequently used services via dependency injection. It also provides helper methods for
    /// displaying tooltips, retrieving user information, checking user roles, and persisting or restoring component
    /// state. All injected services are managed by the framework and should not be disposed manually.</remarks>
    public abstract class SabatexRadzenBlazorBaseComponent : RadzenComponent
    {
        /// <summary>
        /// Gets or sets the HTTP client used to send requests to remote servers.
        /// </summary>
        /// <remarks>This property is typically injected by the framework and should be used to perform
        /// HTTP operations such as GET, POST, PUT, and DELETE within the component. Do not dispose of this instance
        /// manually.</remarks>
        [Inject]
        protected HttpClient HttpClient { get; set; } = default!;
        /// <summary>
        /// Gets or sets the <see cref="NavigationManager"/> used for managing URI navigation and location state within
        /// the application.
        /// </summary>
        /// <remarks>This property is typically provided by dependency injection in Blazor components. Use
        /// it to programmatically navigate to different URIs or to access information about the current navigation
        /// state.</remarks>
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        /// <summary>
        /// Gets or sets the service used to send notifications to the user interface.
        /// </summary>
        [Inject]
        protected NotificationService NotificationService { get; set; } = default!;
        /// <summary>
        /// Gets or sets the JavaScript interop service for interacting with client-side functionality.
        /// </summary>
        [Inject]
        protected SabatexJsInterop SabatexJS {get;set;}=default!;
        /// <summary>
        /// Gets or sets the provider used to obtain authentication state information for the current user.
        /// </summary>
        /// <remarks>This property is typically injected by the Blazor framework to enable components to
        /// query or respond to changes in user authentication state. Use this provider to access authentication data or
        /// subscribe to authentication state changes within the component.</remarks>
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        /// <summary>
        /// Gets or sets the service used to display tooltips within the component.
        /// </summary>
        [Inject] protected TooltipService TooltipService { get; set; } = default!;
        /// <summary>
        /// Gets or sets the service used to manage component state across the application.
        /// </summary>
        /// <remarks>This property is typically set by dependency injection. Use this service to persist
        /// and retrieve state information for components.</remarks>
        [Inject] protected IComponentStateStore ComponentStateStore { get; set; } = default!;
        /// <summary>
        /// Displays a tooltip with the specified message for the given element.
        /// </summary>
        /// <param name="elementReference">A reference to the element for which the tooltip will be shown.</param>
        /// <param name="message">The message to display inside the tooltip.</param>
        /// <param name="options">An optional set of configuration options that control the appearance and behavior of the tooltip. If null,
        /// default options are used.</param>
        protected void ShowTooltip(ElementReference elementReference, string message, TooltipOptions? options = null) => TooltipService.Open(elementReference, message, options);

        /// <summary>
        /// Asynchronously retrieves the unique identifier of the currently authenticated user.
        /// </summary>
        /// <remarks>This method relies on the presence of a claim of type <see
        /// cref="System.Security.Claims.ClaimTypes.NameIdentifier"/> in the current user's authentication state. If the
        /// claim is absent, the method returns <see cref="Guid.Empty"/>.</remarks>
        /// <returns>A <see cref="Guid"/> representing the user's unique identifier. Returns <see cref="Guid.Empty"/> if the user
        /// is not authenticated or the identifier claim is missing.</returns>
        protected async Task<Guid> GetUserIdAsync()
        {
            return Guid.Parse((await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        } 

        /// <summary>
        /// Determines whether the current user is a member of the specified role.
        /// </summary>
        /// <param name="role">The name of the role to check for membership. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
        /// user is in the specified role; otherwise, <see langword="false"/>.</returns>
        protected async Task<bool> UserIsInRore(string role)
        {
            return (await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User.IsInRole(role) ?? false;
        }


        /// <summary>
        /// Saves the current state of the component to the state store using the specified state type.
        /// </summary>
        /// <remarks>Only properties with matching names and compatible accessors between the component
        /// and the state type are copied. The state is saved under a key based on the state type's name. This method
        /// does not persist properties that do not have both readable component and writable state
        /// properties.</remarks>
        /// <typeparam name="TState">The type representing the state to be saved. Must have a parameterless constructor. Properties with matching
        /// names between the component and the state type will be copied.</typeparam>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task SaveState<TState>() where TState : new()
        {
            TState state = new TState();
            var stateProperties = typeof(TState).GetProperties();
            var componentProperties = this.GetType().GetProperties();

            foreach (var stateProp in stateProperties)
            {
                var compProp = componentProperties.FirstOrDefault(p => p.Name == stateProp.Name);
                if (compProp != null && compProp.CanRead && stateProp.CanWrite)
                {
                    var value = compProp.GetValue(this);
                    stateProp.SetValue(state, value);
                }
            }
            await ComponentStateStore.SaveStateAsync(typeof(TState).Name, state);
        }

        /// <summary>
        /// Restores the public writable properties of the current component instance from the persisted state of the
        /// specified type.
        /// </summary>
        /// <remarks>Only properties with matching names and compatible types between the state type and
        /// the component are restored. Properties on the component that do not have a corresponding property in the
        /// state type are not affected.</remarks>
        /// <typeparam name="TState">The type representing the state to restore. The public properties of this type are mapped to matching
        /// writable properties on the component.</typeparam>
        /// <returns>A task that represents the asynchronous restore operation.</returns>
        public  async Task RestoreState<TState>()
        {
            var stateName = typeof(TState).Name;
            var state = await ComponentStateStore.LoadStateAsync<TState>(typeof(TState).Name);

            var stateProperties = typeof(TState).GetProperties();
            var componentProperties = this.GetType().GetProperties();

            foreach (var stateProp in stateProperties)
            {
                var compProp = componentProperties.FirstOrDefault(p => p.Name == stateProp.Name);
                if (compProp != null && compProp.CanWrite)
                {
                    var value = stateProp.GetValue(state);
                    compProp.SetValue(this, value);
                }
            }
        }


    }
}
