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
    public static class SabatexComponentExtension
    {
        public static async Task<string> GetUserIdAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        public static async Task<string> GetUserNameAsync(this AuthenticationStateProvider stateProvider)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.Identity?.Name ?? string.Empty;
        }

        public static async Task<bool> IsInRoreAsync(this AuthenticationStateProvider stateProvider,string role)
        {
            return (await stateProvider.GetAuthenticationStateAsync())?.User.IsInRole(role) ?? false;
        }
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
