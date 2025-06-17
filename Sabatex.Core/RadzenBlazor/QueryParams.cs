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
    /// <summary>
    /// Gets or sets the number of items to skip. Typically used for paging scenarios to specify the offset of items to
    /// exclude from the beginning of a collection.
    /// </summary>
    public int? Skip { get; set; }
    /// <summary>
    /// Gets or sets the maximum number of items to take in a query, typically used for paging.
    /// </summary>
    public int? Top { get; set; }
    /// <summary>
    /// Gets or sets the sort expression used to order query results.
    /// </summary>
    /// <remarks>The format of the sort expression should follow the conventions of the underlying query
    /// system. Ensure that the specified fields exist in the data source to avoid runtime errors.</remarks>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the filter expression used to determine which items are included in the operation.
    /// </summary>
    public string? Filter { get; set; }

    public ForeginKey? ForeginKey { get; set; }
    /// <summary>
    /// Include nested entity
    /// </summary>
    public IEnumerable<string> Include { get; set; }
    public QueryParams(ForeginKey? foreginKey = null)
    {
        ForeginKey = foreginKey;
    }
 
}