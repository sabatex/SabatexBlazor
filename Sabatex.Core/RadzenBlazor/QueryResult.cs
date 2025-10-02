using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sabatex.Core.RadzenBlazor;

/// <summary>
/// Class ODataServiceResult.
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueryResult<T>
{
    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public IEnumerable<T> Value { get; set; }
}