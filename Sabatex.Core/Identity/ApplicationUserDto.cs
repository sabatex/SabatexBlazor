using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.Identity;
/// <summary>
/// Represents a data transfer object (DTO) for application user information.
/// </summary>
/// <remarks>This class is used to encapsulate user identity details, such as the unique identifier, full name, 
/// email address, and optional phone number, for transfer between application layers or external systems.</remarks>
public class ApplicationUserDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the user identity (do not set).
    /// </summary>
    public string Id { get; set; } = default!;
    /// <summary>
    /// Gets or sets the full name of the user identity.
    /// </summary>
    public string FullName { get; set; } = default!;
    /// <summary>
    /// Gets or sets the email address associated with the user identity.
    /// </summary>
    public string Email { get; set; } = default!;
    /// <summary>
    /// Gets or sets the phone number associated with the user identity.
    /// </summary>
    public string? PhoneNumber { get; set; }
}
