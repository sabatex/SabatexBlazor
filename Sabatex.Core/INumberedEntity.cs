using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;

/// <summary>
/// Defines an entity that optionally carries a document number for display or registration purposes.
/// </summary>
public interface INumberedEntity
{
    /// <summary>
    /// Optional document number assigned either automatically or manually.
    /// May be <c>null</c> for drafts or unregistered documents.
    /// </summary>
    string? Number { get; set; }
}
