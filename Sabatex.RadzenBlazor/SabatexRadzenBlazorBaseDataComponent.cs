using Microsoft.AspNetCore.Components;
using Radzen;
using Sabatex.Core.RadzenBlazor;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;


namespace Sabatex.RadzenBlazor;
/// <summary>
/// Provides a base class for Radzen Blazor components that require data access through a configurable data adapter.
/// </summary>
/// <remarks>Inherit from this class to create custom Radzen Blazor components that interact with data sources
/// using an ISabatexRadzenBlazorDataAdapter. The data adapter is typically provided via dependency injection.</remarks>
public abstract class SabatexRadzenBlazorBaseDataComponent: SabatexRadzenBlazorBaseComponent 
{
    /// <summary>
    /// Gets or sets the data adapter used to interact with Radzen Blazor data sources.
    /// </summary>
    /// <remarks>This property is typically set by dependency injection. Assign an implementation of
    /// ISabatexRadzenBlazorDataAdapter to enable data operations within the component.</remarks>
   [Inject]
    protected ISabatexRadzenBlazorDataAdapter DataAdapter { get; set; } = default!;
   
}
