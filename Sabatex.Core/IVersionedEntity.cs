using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;

/// <summary>
/// Defines a version-aware entity that tracks its last persisted state using a timestamp.
/// </summary>
public interface IVersionedEntity
{
    /// <summary>
    /// Timestamp that is updated on each save operation.
    /// Used for detecting concurrent modifications and controlling optimistic concurrency.
    /// </summary>
    DateTimeOffset DateStamp { get; set; }
}

