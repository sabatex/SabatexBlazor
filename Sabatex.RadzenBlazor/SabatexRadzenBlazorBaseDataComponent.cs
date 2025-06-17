using Microsoft.AspNetCore.Components;
using Radzen;
using Sabatex.Core.RadzenBlazor;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;


namespace Sabatex.RadzenBlazor;

public abstract class SabatexRadzenBlazorBaseDataComponent<TKey>: SabatexRadzenBlazorBaseComponent 
{
   [Inject]
    protected ISabatexRadzenBlazorDataAdapter<TKey> DataAdapter { get; set; } = default!;
   
}
