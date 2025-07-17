using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;

/// <summary>
/// Defines an entity that exposes a catalog code used for identification and indexing.
/// </summary>
public interface ICatalogCode
{
    /// <summary>
    /// Catalog code assigned to the object (e.g., SKU, reference number, article).
    /// Typically unique within the catalog or its hierarchy.
    /// </summary>
    string? Code { get; set; }
}
