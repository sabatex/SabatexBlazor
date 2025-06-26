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
    public abstract class SabatexRadzenBlazorBaseComponent : RadzenComponent
    {
        [Inject]
        protected HttpClient HttpClient { get; set; } = default!;
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject]
        protected NotificationService NotificationService { get; set; } = default!;
        [Inject]
        protected SabatexJsInterop SabatexJS {get;set;}=default!;
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
        [Inject] protected TooltipService TooltipService { get; set; } = default!;

        [Inject] protected IComponentStateStore ComponentStateStore { get; set; } = default!;

        protected void ShowTooltip(ElementReference elementReference, string message, TooltipOptions options = null) => TooltipService.Open(elementReference, message, options);


        protected async Task<Guid> GetUserIdAsync()
        {
            return Guid.Parse((await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        } 

        protected async Task<bool> UserIsInRore(string role)
        {
            return (await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User.IsInRole(role) ?? false;
        }


        /// <summary>
        /// Створює об'єкт стану з даних компонента. 
        /// Для кожної властивості в TState, яка збігається з властивістю компонента за ім'ям, записує її значення.
        /// </summary>
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
        /// Застосовує дані стану до компоненту.
        /// Для кожної властивості в TState, яка збігається за ім'ям із властивістю компоненту, встановлює її значення.
        /// </summary>
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
