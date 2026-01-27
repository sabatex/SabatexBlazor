using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    /// <summary>
    /// Provides extension methods for working with authentication state and component state management in Blazor
    /// applications.
    /// </summary>
    /// <remarks>This static class includes helper methods for retrieving user information from an
    /// AuthenticationStateProvider and for saving or restoring component state using a state store. The extension
    /// methods are intended to simplify common authentication and state management tasks in Blazor
    /// components.</remarks>
    public static class SabatexComponentExtension
    {
        /// <summary>
        /// Asynchronously retrieves the unique identifier of the current authenticated user.
        /// </summary>
        /// <remarks>This method extracts the user identifier from the authentication state using the
        /// NameIdentifier claim. If the user is not authenticated or the claim is missing, the method returns an empty
        /// string.</remarks>
        /// <param name="stateProvider">The authentication state provider used to obtain the current user's authentication state. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user's unique identifier as
        /// a string, or an empty string if the user is not authenticated or the identifier is unavailable.</returns>
        public static async Task<string> GetUserIdAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        /// <summary>
        /// Asynchronously retrieves the user name associated with the current authentication state.
        /// </summary>
        /// <remarks>If the user is not authenticated or the authentication state does not provide a user
        /// name, the method returns an empty string.</remarks>
        /// <param name="stateProvider">The authentication state provider used to obtain the current user's authentication information. Cannot be
        /// null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user name if the user is
        /// authenticated; otherwise, an empty string.</returns>
        public static async Task<string> GetUserNameAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.Identity?.Name ?? string.Empty;
        }
        /// <summary>
        /// Asynchronously determines whether the current user is a member of the specified role.
        /// </summary>
        /// <remarks>If the authentication state is unavailable or the user is not authenticated, the
        /// method returns <see langword="false"/>.</remarks>
        /// <param name="stateProvider">The authentication state provider used to retrieve the current user's authentication state. Cannot be null.</param>
        /// <param name="role">The name of the role to check for membership. Cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
        /// current user is in the specified role; otherwise, <see langword="false"/>.</returns>
        public static async Task<bool> IsInRoreAsync(this AuthenticationStateProvider stateProvider,string role)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.IsInRole(role) ?? false;
        }
        /// <summary>
        /// Asynchronously determines whether the current user is authenticated.
        /// </summary>
        /// <remarks>This method provides a convenient way to check the authentication status of the
        /// current user in Blazor applications. It returns <see langword="false"/> if the authentication state or user
        /// identity is unavailable.</remarks>
        /// <param name="stateProvider">The authentication state provider used to retrieve the current authentication state. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
        /// user is authenticated; otherwise, <see langword="false"/>.</returns>
        public static async Task<bool> IsAuthenticatedAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.Identity?.IsAuthenticated ?? false;
        }

        ///// <summary>
        ///// Створює об'єкт стану з даних компонента. 
        ///// Для кожної властивості в TState, яка збігається з властивістю компонента за ім'ям, записує її значення.
        ///// </summary>
        //public static async Task SaveState<TState>(this SabatexRadzenBlazorBaseComponent component,IComponentStateStore stateStore) where TState : new()
        //{
        //    TState state = new TState();
        //    var stateProperties = typeof(TState).GetProperties();
        //    var componentProperties = component.GetType().GetProperties();

        //    foreach (var stateProp in stateProperties)
        //    {
        //        var compProp = componentProperties.FirstOrDefault(p => p.Name == stateProp.Name);
        //        if (compProp != null && compProp.CanRead && stateProp.CanWrite)
        //        {
        //            var value = compProp.GetValue(component);
        //            stateProp.SetValue(state, value);
        //        }
        //    }
        //    await stateStore.SaveStateAsync(typeof(TState).Name, state);
        //}

        ///// <summary>
        ///// Застосовує дані стану до компоненту.
        ///// Для кожної властивості в TState, яка збігається за ім'ям із властивістю компоненту, встановлює її значення.
        ///// </summary>
        //public static async Task  RestoreState<TState>(this SabatexRadzenBlazorBaseComponent component, IComponentStateStore stateStore)
        //{
        //    var stateName = typeof(TState).Name;
        //    var state = await stateStore.LoadStateAsync<TState>(typeof(TState).Name);

        //    var stateProperties = typeof(TState).GetProperties();
        //    var componentProperties = component.GetType().GetProperties();

        //    foreach (var stateProp in stateProperties)
        //    {
        //        var compProp = componentProperties.FirstOrDefault(p => p.Name == stateProp.Name);
        //        if (compProp != null && compProp.CanWrite)
        //        {
        //            var value = stateProp.GetValue(state);
        //            compProp.SetValue(component, value);
        //        }
        //    }
        //}


    }
}
