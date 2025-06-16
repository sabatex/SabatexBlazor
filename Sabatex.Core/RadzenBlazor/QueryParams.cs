using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.RadzenBlazor;

/// <summary>
/// QueryParameters for IDataAdapter 
/// </summary>
public class QueryParams
{
    public LoadDataArgs Args { get; set; }
    /// <summary>
    /// Inform query for used foregin key
    /// </summary>
    public ForeginKey? ForeginKey { get; set; }
    /// <summary>
    /// Include nested entity
    /// </summary>
    public IEnumerable<string> Include { get; set; }
    public QueryParams(LoadDataArgs args,ForeginKey? foreginKey = null)
    {
        Args = args; 
        ForeginKey = foreginKey;
    }
    public QueryParams()
    {
        Args = new LoadDataArgs();
    }
}