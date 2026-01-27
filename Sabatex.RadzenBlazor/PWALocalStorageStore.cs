using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// Provides a component state store implementation that persists state using the browser's local storage via JavaScript
/// interop.
/// </summary>
/// <remarks>This class enables Blazor components to save and retrieve state data in the browser's local storage,
/// allowing state to persist across page reloads and browser sessions. State is serialized to JSON using camel case
/// property naming. This implementation requires a valid JavaScript runtime and is intended for use in Progressive Web
/// App (PWA) scenarios or other client-side Blazor applications.</remarks>
public class PWALocalStorageStore : IComponentStateStore
{
    private readonly IJSRuntime _js;
    /// <summary>
    /// Initializes a new instance of the PWALocalStorageStore class using the specified JavaScript runtime.
    /// </summary>
    /// <remarks>This constructor enables the PWALocalStorageStore to perform local storage operations within
    /// a Progressive Web App (PWA) context by leveraging the provided IJSRuntime instance.</remarks>
    /// <param name="js">The JavaScript runtime interface used to interact with browser local storage.</param>
    public PWALocalStorageStore(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Asynchronously loads and deserializes the state object associated with the specified key from browser local
    /// storage.
    /// </summary>
    /// <remarks>If the specified key does not exist in local storage or the stored value is null, the method
    /// returns <see langword="null"/>. The method uses JSON serialization; ensure that <typeparamref name="TState"/> is
    /// compatible with JSON serialization and deserialization.</remarks>
    /// <typeparam name="TState">The type of the state object to deserialize and return.</typeparam>
    /// <param name="key">The key that identifies the state object in local storage. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized state object of
    /// type <typeparamref name="TState"/> if the key exists; otherwise, <see langword="null"/>.</returns>
    public async Task<TState?> LoadStateAsync<TState>(string key)
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", key);
        return json is not null
            ? JsonSerializer.Deserialize<TState>(json)
            : default;
    }
    /// <summary>
    /// Asynchronously saves the specified state object to browser local storage under the given key.
    /// </summary>
    /// <remarks>If an entry with the specified key already exists in local storage, it will be overwritten.
    /// The state object is serialized to JSON using camel case property names. This method does not perform validation
    /// on the state object; ensure that the object is serializable.</remarks>
    /// <typeparam name="TState">The type of the state object to be serialized and stored.</typeparam>
    /// <param name="key">The key under which the state will be stored in local storage. Cannot be null or empty.</param>
    /// <param name="state">The state object to serialize and save. All public properties will be serialized using camel case naming.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async Task SaveStateAsync<TState>(string key, TState state)
    {
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await _js.InvokeVoidAsync("localStorage.setItem", key, json);

    }
}
