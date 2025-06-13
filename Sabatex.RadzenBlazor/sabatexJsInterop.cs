using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sabatex.RadzenBlazor;

// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

public class SabatexJsInterop : IAsyncDisposable
{
    public class WindowDimensions
    {
        public int height { get; set; }
        public int width { get; set; }
        public int availHeight { get; set; }
        public int availWidth { get; set; }
    };
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public SabatexJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import",
            $"./_content/Sabatex.RadzenBlazor/Sabatex.RadzenBlazor.js?v={(typeof(PWAPushService).Assembly.GetName().Version)}").AsTask());
    }

    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("sabatex.showPrompt", message);
    }
    public async ValueTask FocusElement(string elementReference)
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<string>("sabatex.focusElement", elementReference);
    }

    public async ValueTask DownloadStringAsFile(string fileName, string text)
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<object>("sabatex.downloadStrigAsFile", fileName, text);
    }

    public async ValueTask<double> GetElementClientHeight(ElementReference element)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("sabatex.getElementClientHeight", element);
    }
    public async ValueTask<double> GetElementOffSetHeight(ElementReference element)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("sabatex.getElementOffSetHeight", element);
    }
    public async ValueTask<double> GetAvailHeight(ElementReference element)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("sabatex.getAvailHeight", element);
    }

    public async ValueTask<bool> IsMomibileDeviceAsync()
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<bool>("sabatex.isDevice");
    }

    public async ValueTask<WindowDimensions> GetWindowDimensionsAsync()
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<WindowDimensions>("sabatex.getWindowDimensions");

    }

    public async ValueTask RadzenBlazorSSRLayout()
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<object>("sabatex.radzenBlazorSSRLayout");
    }
    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    public async ValueTask<string> CleanCookie(string cookieName)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("sabatex.cleanCookie", cookieName);
    }
}