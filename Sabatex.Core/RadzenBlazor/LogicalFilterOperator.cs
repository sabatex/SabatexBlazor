using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.RadzenBlazor;

/// <summary>
/// Specifies the logical operator between filters.
/// </summary>
public enum LogicalFilterOperator
{
    /// <summary>
    /// All filters should be satisfied.
    /// </summary>
    And,
    /// <summary>
    /// Any filter should be satisfied.
    /// </summary>
    Or
}