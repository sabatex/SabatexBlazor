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
        [Inject] private TooltipService tooltipService { get; set; } = default!;
        protected void ShowTooltip(ElementReference elementReference, string message, TooltipOptions options = null) => tooltipService.Open(elementReference, message, options);


        protected async Task<Guid> GetUserIdAsync()
        {
            return Guid.Parse((await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        } 

        protected async Task<bool> UserIsInRore(string role)
        {
            return (await AuthenticationStateProvider.GetAuthenticationStateAsync())?.User.IsInRole(role) ?? false;
        }

    }
}
