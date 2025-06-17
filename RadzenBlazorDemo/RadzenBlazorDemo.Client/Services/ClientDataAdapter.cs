using Microsoft.AspNetCore.Components;
using Sabatex.Bakery.Client;
using Sabatex.RadzenBlazor;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Sabatex.Bakery.Client.Services;

public class ClientDataAdapter: SabatexRadzenBlazorApiDataAdapter<Guid>
{
    public ClientDataAdapter(HttpClient httpClient, ILogger<SabatexRadzenBlazorODataAdapter<Guid>> logger, NavigationManager navigationManager) : base(httpClient, logger, navigationManager)
    {
    }


}
