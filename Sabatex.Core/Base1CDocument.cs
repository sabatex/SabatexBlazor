using sabatex.BakeryWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;

/// <summary>
/// Abstract marker class representing a generic document.
/// Serves as a base for business documents that require document semantics (e.g., signing, posting).
/// </summary>
public abstract class Base1CDocument : Base1CObject
{
    /// <summary>
    /// Logical document date (e.g., accounting or transaction date).
    /// Must be explicitly assigned during document creation or registration.
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Indicates whether the document has been posted (finalized in workflow or accounting logic).
    /// </summary>
    public bool Posted { get; set; }
}
