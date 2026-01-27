using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// Represents the essential metadata for a Progressive Web App (PWA), including manifest and icon resources.
/// </summary>
/// <remarks>This record is typically used to encapsulate the key resources required for PWA installation and
/// cross-platform icon support.</remarks>
/// <param name="Manifest">The URL or path to the web app manifest file, which defines the app's appearance and behavior when installed.</param>
/// <param name="AppleTouchIcon">The URL or path to the Apple touch icon, used when the app is added to the home screen on iOS devices.</param>
/// <param name="Icon">The URL or path to the primary icon representing the app across platforms.</param>
public record PWADescriptor(string Manifest, string AppleTouchIcon, string Icon);
/// <summary>
/// Represents a WebAssembly (WASM) client application, including its assembly, route prefix, and optional Progressive
/// Web App (PWA) configuration.
/// </summary>
/// <remarks>Use the WASMClient class to register and manage WebAssembly client applications within the current
/// environment. Each instance encapsulates the application's assembly, routing information, and optional PWA metadata.
/// The static WASMClients collection maintains all registered clients for the application lifecycle.</remarks>
public class WASMClient
{
    /// <summary>
    /// Gets the assembly WASMClient instance.
    /// </summary>
    public Assembly Assembly { get; init; }
    /// <summary>
    /// Gets the route prefix that is applied to all endpoints in the WASMClient.
    /// </summary>
    public string PrefixRoute { get; init; } 
    
    /// <summary>
    /// Gets or sets the Progressive Web App (PWA) descriptor for the application.
    /// </summary>
    public PWADescriptor? PWA { get; init; }
    /// <summary>
    /// Gets or sets a value indicating whether the content is authorized.
    /// </summary>
    public bool AuthorizedContent { get; set; }
    /// <summary>
    /// Initializes a new instance of the WASMClient class with the specified assembly, route prefix, and Progressive
    /// Web App (PWA) descriptor.
    /// </summary>
    /// <param name="Assembly">The assembly containing the application's entry point and resources. Cannot be null.</param>
    /// <param name="authorizedContent"></param>
    /// <param name="PrefixRoute">The base route prefix for the application. If null, a default prefix is generated from the assembly name.</param>
    /// <param name="pWA">An optional descriptor containing Progressive Web App (PWA) configuration. May be null if PWA features are not
    /// required.</param>
    /// <exception cref="ArgumentNullException">Thrown if PrefixRoute is null and the assembly name cannot be determined.</exception>
    public WASMClient(Assembly Assembly, string? PrefixRoute, PWADescriptor? pWA, bool authorizedContent = false)
    {
        this.Assembly = Assembly;
        this.PrefixRoute = PrefixRoute == null 
            ? Assembly.GetName().Name?.Replace('.', '_') ?? throw new ArgumentNullException(nameof(PrefixRoute)) 
            : PrefixRoute;
        this.PrefixRoute = this.PrefixRoute.StartsWith('/') ? this.PrefixRoute : "/" + this.PrefixRoute;
        this.PWA = pWA;
        this.AuthorizedContent = authorizedContent; 
    }
    /// <summary>
    /// Gets the collection of active WebAssembly clients.
    /// </summary>
    public static List<WASMClient> WASMClients { get; } = new List<WASMClient>();
    /// <summary>
    /// Registers a WebAssembly (WASM) client application for use within the current environment.
    /// </summary>
    /// <param name="Assembly">The assembly containing the WASM client application to register. Cannot be null.</param>
    /// <param name="prefixRoute">An optional route prefix to use for the WASM client. If specified, the client will be accessible under this
    /// route.</param>
    /// <param name="pwa">An optional progressive web app (PWA) descriptor that defines PWA-related metadata for the client. If null, no
    /// PWA configuration is applied.</param>
    /// <param name="authorizedContent"></param>
    public static void AddWASMClient(Assembly Assembly, string? prefixRoute, PWADescriptor? pwa = null, bool authorizedContent = false)
    {
        WASMClients.Add(new WASMClient(Assembly, prefixRoute, pwa, authorizedContent));
    }
}
