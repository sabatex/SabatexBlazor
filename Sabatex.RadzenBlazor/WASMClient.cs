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
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class WASMClientAttribute : Attribute
{
    /// <summary>
    /// Gets the route prefix that is applied to all endpoints in the WASMClient.
    /// </summary>
    public string PrefixRoute { get; init; }

    /// <summary>
    /// Gets or sets the manifest content associated with the current instance.
    /// </summary>
    public string? Manifest { get; set; }
    
    /// <summary>
    /// Gets or sets the URL of the Apple Touch icon to be used for web applications on iOS devices.
    /// </summary>
    /// <remarks>Set this property to specify a custom icon that appears when users add the web application to
    /// their iOS home screen. If not set, no Apple Touch icon will be provided.</remarks>
    public string? AppleTouchIcon { get; set; }
    /// <summary>
    /// Gets or sets the URL of the primary icon for the web application.
    /// </summary>
    public string? Icon { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the content is authorized.
    /// </summary>
    public bool AuthorizedContent { get; set; }
    /// <summary>
    /// Initializes a new instance of the WASMClient class with the specified assembly, route prefix, and Progressive
    /// Web App (PWA) descriptor.
    /// </summary>
    /// <param name="Assembly">The assembly containing the application's entry point and resources. Cannot be null.</param>
    /// <param name="PrefixRoute">The base route prefix for the application. If null, a default prefix is generated from the assembly name.</param>
    /// <param name="manifest">The URL or path to the web app manifest file, which defines the app's appearance and behavior when installed.</param>
    /// <param name="appleTouchIcon">The URL or path to the Apple touch icon, used when the app is added to the home screen on iOS devices.</param>
    /// <param name="icon">The URL or path to the primary icon representing the app across platforms.</param>
    /// <param name="authorizedContent"></param>
    /// <exception cref="ArgumentNullException">Thrown if PrefixRoute is null and the assembly name cannot be determined.</exception>
    public WASMClientAttribute(string? PrefixRoute, string? manifest, string? appleTouchIcon, string? icon, bool authorizedContent = false)
    {
        //this.Assembly = Assembly;
        //this.PrefixRoute = PrefixRoute == null 
        //    ? Assembly.GetName().Name?.Replace('.', '_') ?? throw new ArgumentNullException(nameof(PrefixRoute)) 
        //    : PrefixRoute;
        this.PrefixRoute = PrefixRoute;
        this.Manifest = manifest;
        this.AppleTouchIcon = appleTouchIcon;
        this.Icon = icon;
        this.AuthorizedContent = authorizedContent; 
    }
    /// <summary>
    /// Gets the collection of active WebAssembly clients.
    /// </summary>
    public static List<WASMClientAttribute> WASMClients { get; } = new List<WASMClientAttribute>();
    ///// <summary>
    ///// Registers a WebAssembly (WASM) client application for use within the current environment.
    ///// </summary>
    ///// <param name="Assembly">The assembly containing the WASM client application to register. Cannot be null.</param>
    ///// <param name="prefixRoute">An optional route prefix to use for the WASM client. If specified, the client will be accessible under this
    ///// route.</param>
    ///// <param name="pwa">An optional progressive web app (PWA) descriptor that defines PWA-related metadata for the client. If null, no
    ///// PWA configuration is applied.</param>
    ///// <param name="authorizedContent"></param>
    //public static void AddWASMClient(Assembly Assembly, string? prefixRoute, PWADescriptor? pwa = null, bool authorizedContent = false)
    //{
    //    WASMClients.Add(new WASMClientAttribute(Assembly, prefixRoute, pwa, authorizedContent));
    //}
    public static List<Assembly> GetWASMClientAssemblies(params Assembly[] assembliesToScan)
    {
        var result = new List<Assembly>();

        foreach (var assembly in assembliesToScan)
        {
            var typesWithAttribute = assembly.GetTypes()
                .Where(t => Attribute.GetCustomAttribute(t, typeof(WASMClientAttribute)) is WASMClientAttribute)
                .ToList();

            if (typesWithAttribute.Count > 1)
            {
                throw new InvalidOperationException(
                    $"Assembly '{assembly.GetName().Name}' includes {typesWithAttribute.Count} types with [WASMClient]. Атрибут може бути застосований лише один раз на assembly.");
            }

            if (typesWithAttribute.Count == 1)
            {
                result.Add(assembly);
            }
        }

        return result;
    }
    /// <summary>
    /// Determines whether the specified path matches any registered WebAssembly (WASM) route prefix.
    /// </summary>
    /// <param name="path">The request path to evaluate against the set of WASM route prefixes. Cannot be null.</param>
    /// <returns>true if the path starts with any WASM route prefix; otherwise, false.</returns>
    public static bool IsWASMRoute(string path) => WASMClients.Any(s => path.StartsWith(s.PrefixRoute));


}
