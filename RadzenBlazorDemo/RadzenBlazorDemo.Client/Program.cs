using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sabatex.Bakery.Client.Services;
using Sabatex.Core.RadzenBlazor;
using Sabatex.RadzenBlazor;

namespace RadzenBlazorDemo.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddAuthenticationStateDeserialization();
            builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
            builder.Services.AddSabatexRadzenBlazor();
            builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            
            builder.Services.AddScoped<ISabatexRadzenBlazorDataAdapter<Guid>, ClientDataAdapter>();

            await builder.Build().RunAsync();
        }
    }
}
