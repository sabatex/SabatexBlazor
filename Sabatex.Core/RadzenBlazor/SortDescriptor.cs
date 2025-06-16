using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.RadzenBlazor;

/// <summary>
/// Represents a sorting description. Used in components that support sorting.
/// </summary>
public class SortDescriptor
{
    /// <summary>
    /// Gets or sets the property to sort by.
    /// </summary>
    /// <value>The property.</value>
    public string Property { get; set; }
    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    /// <value>The sort order.</value>
    public SortOrder? SortOrder { get; set; }
}