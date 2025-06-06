using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Sabatex.RadzenBlazor;
using Sabatex.RadzenBlazor.Demo;
using Sabatex.RadzenBlazor.Demo.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSabatexRadzenBlazor();
builder.Services.AddScoped<ISabatexRadzenBlazorDataAdapter<Guid>, DataAdapter>();

await builder.Build().RunAsync();
