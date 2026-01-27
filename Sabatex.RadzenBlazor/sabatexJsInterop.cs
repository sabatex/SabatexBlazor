using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Sabatex.RadzenBlazor;

/// <summary>
/// Provides JavaScript interop methods for browser-related operations in Blazor applications, such as displaying
/// prompts, manipulating elements, handling file downloads, and retrieving window or device information.
/// </summary>
/// <remarks>This class encapsulates calls to JavaScript functions defined in the Sabatex.RadzenBlazor.js module,
/// enabling seamless integration of browser features within Blazor components. Methods are asynchronous and should be
/// awaited to ensure proper execution. The class implements IAsyncDisposable; call DisposeAsync to release JavaScript
/// resources when the instance is no longer needed.</remarks>
public class SabatexJsInterop : IAsyncDisposable
{
    /// <summary>
    /// Represents the dimensions of a window, including its total and available width and height.
    /// </summary>
    public class WindowDimensions
    {
        /// <summary>
        /// Gets or sets the height value.
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// Gets or sets the width value.
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// Gets or sets the height, in pixels, of the content area available for rendering, excluding interface
        /// features such as toolbars and scrollbars.
        /// </summary>
        public int availHeight { get; set; }
        /// <summary>
        /// Gets or sets the width, in pixels, of the content area available for rendering, excluding interface features
        /// such as scrollbars.
        /// </summary>
        public int availWidth { get; set; }
    };
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    /// <summary>
    /// Initializes a new instance of the SabatexJsInterop class using the specified JavaScript runtime.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime instance used to invoke JavaScript functions from .NET code. Cannot be null.</param>
    public SabatexJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import",
            $"./_content/Sabatex.RadzenBlazor/Sabatex.RadzenBlazor.js?v={(typeof(PWAPushService).Assembly.GetName().Version)}").AsTask());
    }

    /// <summary>
    /// Displays a prompt dialog to the user and returns the entered text.
    /// </summary>
    /// <param name="message">The message to display in the prompt dialog. This text is shown to the user as the prompt's instruction or
    /// question.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the text entered by the user, or an
    /// empty string if the user cancels the prompt.</returns>
    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("sabatex.showPrompt", message);
    }
    /// <summary>
    /// Sets focus to the specified HTML element in the browser.
    /// </summary>
    /// <remarks>Use this method to programmatically move focus to a specific element, such as an input or
    /// button, in a Blazor application. This can be useful for improving accessibility or guiding user
    /// interaction.</remarks>
    /// <param name="elementReference">The identifier of the HTML element to receive focus. This should correspond to a valid element reference in the
    /// current page.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask FocusElement(string elementReference)
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<string>("sabatex.focusElement", elementReference);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public async ValueTask DownloadStringAsFile(string fileName, string text)
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<object>("sabatex.downloadStrigAsFile", fileName, text);
    }
    /// <summary>
    /// Asynchronously retrieves the client height of the specified HTML element.
    /// </summary>
    /// <param name="element">A reference to the HTML element whose client height is to be obtained.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the client height, in pixels, of the
    /// specified element.</returns>
    public async ValueTask<double> GetElementClientHeight(ElementReference element)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("sabatex.getElementClientHeight", element);
    }
    /// <summary>
    /// Asynchronously retrieves the offset height of the specified HTML element.
    /// </summary>
    /// <param name="element">A reference to the HTML element for which to obtain the offset height. Must be a valid, rendered element.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the offset height of the element, in
    /// pixels.</returns>
    public async ValueTask<double> GetElementOffSetHeight(ElementReference element)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("sabatex.getElementOffSetHeight", element);
    }
    /// <summary>
    /// Asynchronously retrieves the available vertical height of the specified HTML element.
    /// </summary>
    /// <param name="element">A reference to the HTML element for which to determine the available height.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the available height, in pixels, of
    /// the specified element.</returns>
    public async ValueTask<double> GetAvailHeight(ElementReference element)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<double>("sabatex.getAvailHeight", element);
    }
    /// <summary>
    /// Asynchronously determines whether the current device is a mobile device.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
    /// current device is identified as a mobile device; otherwise, <see langword="false"/>.</returns>
    public async ValueTask<bool> IsMomibileDeviceAsync()
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<bool>("sabatex.isDevice");
    }
    /// <summary>
    /// Asynchronously retrieves the current dimensions of the browser window.
    /// </summary>
    /// <returns>A <see cref="WindowDimensions"/> object containing the width and height of the browser window.</returns>
    public async ValueTask<WindowDimensions> GetWindowDimensionsAsync()
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<WindowDimensions>("sabatex.getWindowDimensions");

    }
    
    /// <summary>
    /// Initializes or updates the server-side rendering layout for Radzen Blazor components.
    /// </summary>
    /// <remarks>Call this method to ensure the Radzen Blazor SSR layout is properly set up or refreshed. This
    /// method should be awaited to ensure layout initialization completes before further operations that depend on the
    /// layout.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask RadzenBlazorSSRLayout()
    {
        var module = await moduleTask.Value;
        await module.InvokeAsync<object>("sabatex.radzenBlazorSSRLayout");
    }
    /// <summary>
    /// Asynchronously releases the unmanaged resources used by the object.
    /// </summary>
    /// <remarks>Call this method to release resources when the object is no longer needed. This method should
    /// be awaited to ensure that all resources are released before continuing execution.</remarks>
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
    /// Asynchronously removes a cookie with the specified name.
    /// </summary>
    /// <param name="cookieName"></param>
    /// <returns></returns>    
    public async ValueTask<string> CleanCookie(string cookieName)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("sabatex.cleanCookie", cookieName);
    }
}