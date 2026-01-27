using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Components;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// Provides application-wide state management for Blazor applications, including page header and content area height
/// information.
/// </summary>
/// <remarks>This class enables components to share and react to changes in common UI state, such as the current
/// page header or the available height for content. Events are provided to notify subscribers when these values change,
/// allowing for dynamic UI updates. This class is typically registered as a scoped service in Blazor applications to
/// facilitate state sharing across components.</remarks>
public class SabatexBlazorAppState
{
    string pageHeader = string.Empty;
    /// <summary>
    /// Gets or sets the header text displayed at the top of the page.
    /// </summary>
    public string PageHeader { get => pageHeader; set { pageHeader = value; OnHeaderChange?.Invoke(value); } }
    /// <summary>
    /// Occurs when the header value changes.
    /// </summary>
    /// <remarks>Subscribers receive the new header value as a string parameter when the event is raised.
    /// Event handlers should be prepared to handle any valid header value, including null or empty strings if
    /// applicable.</remarks>
    public event Action<string>? OnHeaderChange;

    double contentAvaliableHeight;
    /// <summary>
    /// Occurs when the available height for content changes.
    /// </summary>
    /// <remarks>Subscribers can use this event to respond to layout changes or resizing that affect the
    /// vertical space available for content. The event provides the new available height as a parameter, measured in
    /// device-independent units (typically pixels).</remarks>
    public event Action<double>? OnContentAvaliableHeightChange;
    
    /// <summary>
    /// Gets or sets the available height for displaying content.
    /// </summary>
    public double ContentAvaliableHeight
       {
              get => contentAvaliableHeight;
              set
              {
                     if (contentAvaliableHeight != value)
                     {
                            contentAvaliableHeight = value;
                            OnContentAvaliableHeightChange?.Invoke(value);
                     }
              }
       }


}