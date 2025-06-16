using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.RadzenBlazor;

/// <summary>
/// Provides data for the <c>Radzen.PagedDataBoundComponent&lt;T&gt;.LoadData</c> event,  which is raised to supply
/// information about data loading operations such as paging, sorting, and filtering.
/// </summary>
/// <remarks>This class is typically used in data-bound components to handle data operations like paging, sorting,
/// and filtering. It provides details such as the number of items to skip or take, the sort order,  and filter
/// expressions.</remarks>
public class LoadDataArgs
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


    //
    // Summary:
    //     Gets the filter expression as a collection of filter descriptors.
    public IEnumerable<FilterDescriptor> Filters { get; set; }

    //
    // Summary:
    //     Gets the sort expression as a collection of sort descriptors.
    //
    // Value:
    //     The sorts.
    public IEnumerable<SortDescriptor> Sorts { get; set; }
}
