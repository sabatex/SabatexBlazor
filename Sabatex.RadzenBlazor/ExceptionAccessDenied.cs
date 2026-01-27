using System;
using System.Collections.Generic;
using System.Text;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// The exception that is thrown when an operation is attempted without sufficient access rights.
/// </summary>
/// <remarks>Use this exception to indicate that a user or process does not have the necessary permissions to
/// perform the requested action. This exception is typically thrown in security-sensitive contexts where access control
/// is enforced.</remarks>
public class ExceptionAccessDenied : Exception
{
    /// <summary>
    /// Initializes a new instance of the ExceptionAccessDenied class with a default error message indicating that
    /// access is denied.
    /// </summary>
    public ExceptionAccessDenied() : base("Access denied.")
    {
    }
}
