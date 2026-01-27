using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Sabatex.RadzenBlazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;


namespace Sabatex.RadzenBlazor;
/// <summary>
/// Provides push notification subscription management for Progressive Web Apps (PWAs) using JavaScript interop and HTTP
/// APIs.
/// </summary>
/// <remarks>PWAPushService enables subscribing, updating, and clearing push notification subscriptions for PWAs
/// in Blazor applications. It interacts with browser APIs via JavaScript interop and communicates with backend
/// endpoints to manage subscription state. Instances of this service should be disposed asynchronously to release
/// JavaScript resources. This service is typically used in client-side Blazor applications to facilitate push
/// notification workflows.</remarks>
public class PWAPushService : IPWAPush,IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly HttpClient _httpClient;
    record Cert(string cert);
    /// <summary>
    /// Initializes a new instance of the PWAPushService class for managing Progressive Web App push notifications and
    /// related JavaScript interop functionality.
    /// </summary>
    /// <remarks>This constructor sets up the required dependencies for interacting with browser APIs and
    /// performing HTTP operations necessary for push notification management. The provided IJSRuntime instance must
    /// support dynamic module imports.</remarks>
    /// <param name="jsRuntime">The JavaScript runtime interface used to invoke JavaScript functions from .NET code.</param>
    /// <param name="httpClient">The HTTP client used for making network requests related to push notification operations.</param>
    public PWAPushService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", $"./_content/Sabatex.RadzenBlazor/Sabatex.RadzenBlazor.js?v={(typeof(PWAPushService).Assembly.GetName().Version)}").AsTask());
        _httpClient = httpClient;
    }
    /// <summary>
    /// Asynchronously releases resources used by the instance.
    /// </summary>
    /// <remarks>Call this method to clean up resources when the instance is no longer needed. This method
    /// should be awaited to ensure that all resources are released before continuing execution.</remarks>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }

    }
    /// <summary>
    /// Asynchronously retrieves the current push subscription for the Progressive Web App (PWA), if one exists.
    /// </summary>
    /// <remarks>This method communicates with the browser to obtain the push subscription. If the user has
    /// not granted permission or no subscription has been created, the result will be <see langword="null"/>.</remarks>
    /// <returns>A <see cref="PWAPushHandler"/> representing the current push subscription, or <see langword="null"/> if no
    /// subscription is available.</returns>
    public async Task<PWAPushHandler?> GetSubscriptionAsync()
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<PWAPushHandler>("sabatexPWAPush.getSubscription");
    }
    /// <summary>
    /// Subscribes the client to push notifications using the public key from the server.
    /// </summary>
    /// <remarks>This method retrieves the server's public key and registers the client for push
    /// notifications. The subscription information is sent to the server to complete the registration process. The
    /// returned <see cref="PWAPushHandler"/> can be used to manage the push subscription.</remarks>
    /// <returns>A <see cref="PWAPushHandler"/> instance representing the push subscription if successful; otherwise, <see
    /// langword="null"/> if the subscription could not be established.</returns>
    public async Task<PWAPushHandler?> SubscribeAsync()
    {
        var module = await moduleTask.Value;
        var cert = await _httpClient.GetFromJsonAsync<Cert>("/api/push/public_key");
        var subscrtion = await module.InvokeAsync<PWAPushHandler>("sabatexPWAPush.subscribe",cert?.cert);
        await _httpClient.PutAsJsonAsync("api/push/subscribe", subscrtion);
        return subscrtion;
    }
    
    /// <summary>
    /// update subscription
    /// </summary>
    /// <returns></returns>
    public async Task UpdateSubscriptionAsync()
    {
        var subscrtion = await GetSubscriptionAsync();
        if (subscrtion == null) 
        {
            subscrtion = await SubscribeAsync();
        }
        var response = await _httpClient.PutAsJsonAsync("api/push/subscribe", subscrtion);
    }

    /// <summary>
    /// Clear subscription on all devices
    /// </summary>
    /// <returns></returns>
    public async Task ClearSubscriptionAsync()
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<object>("sabatexPWAPush.unsubscribe");
        await _httpClient.PostAsync("api/push/clearSubscribe",null);
    }
    /// <summary>
    /// Asynchronously unsubscribes the current user from push notifications.
    /// </summary>
    /// <remarks>This method removes the user's subscription both from the client and the server. If the user
    /// is not currently subscribed, the operation completes without error.</remarks>
    /// <returns>A task that represents the asynchronous unsubscribe operation.</returns>
    public async Task UnSubscribeAsync()
    {
        var module = await moduleTask.Value;
        var subscrtion = await GetSubscriptionAsync();
        await module.InvokeAsync<object>("sabatexPWAPush.unsubscribe");
        await _httpClient.PutAsJsonAsync("api/push/unsubscribe", subscrtion);

    }
}
