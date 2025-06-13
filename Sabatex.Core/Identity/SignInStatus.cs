using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.Identity;

/// <summary>
/// Represents the result of a sign-in attempt.
/// </summary>
/// <remarks>This enumeration is used to indicate the outcome of a sign-in operation. It provides information
/// about whether the sign-in was successful, requires additional steps, or failed due to specific conditions.</remarks>
public enum SignInStatus
{
    Success,
    RequiresTwoFactor,
    LockedOut,
    InvalidCredentials
}