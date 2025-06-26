using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;

public class PWALocalStorageStore : IComponentStateStore
{
    private readonly IJSRuntime _js;

    public PWALocalStorageStore(IJSRuntime js)
    {
        _js = js;
    }


    public async Task<TState?> LoadStateAsync<TState>(string key)
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
        return json is not null
            ? JsonSerializer.Deserialize<TState>(json)
            : default;
    }

    public async Task SaveStateAsync<TState>(string key, TState state)
    {
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await _js.InvokeVoidAsync("localStorage.setItem", key, json);

    }
}
