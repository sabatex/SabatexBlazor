using Microsoft.AspNetCore.Components;
using Sabatex.Core.Identity;
using Sabatex.RadzenBlazor.Services;

namespace Sabatex.RadzenBlazor.Demo.Services
{
    public class IdentityAdapter : IdentityAdapterWasm
    {
        public IdentityAdapter(HttpClient http, NavigationManager navigationManager) : base(http,navigationManager)
        {
        }

        public override async Task<IEnumerable<ExternalProvider>> GetExternalProvidersAsync()
        {
            return new ExternalProvider[] { new ExternalProvider("Google","Google") ,new ExternalProvider ("Microsoft", "Microsoft") };
        }


    }
}
