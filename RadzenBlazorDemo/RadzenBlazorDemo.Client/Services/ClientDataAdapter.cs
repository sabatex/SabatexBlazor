using Microsoft.AspNetCore.Components;
using Sabatex.Bakery.Client;
using Sabatex.RadzenBlazor;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Sabatex.Bakery.Client.Services;

public class ClientDataAdapter: SabatexRadzenBlazorApiDataAdapter
{
    public ClientDataAdapter(HttpClient httpClient, ILogger<ClientDataAdapter> logger, NavigationManager navigationManager) : base(httpClient, logger, navigationManager)
    {
    }


}
