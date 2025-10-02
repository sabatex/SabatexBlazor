using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.Identity;
/// <summary>
/// Represents the data transfer object used for external user registration (Gmail,Microsoft,...).
/// </summary>
/// <remarks>This record is typically used to capture and transfer information required for registering a user
/// through an external authentication provider. It includes details such as the user's email, name, and
/// provider-specific information.</remarks>
public record ExtermnalRegisterDTO
{
    /// <summary>
    /// Gets or sets the email address associated with the user.
    /// </summary>
    public string Email { get; set; }=string.Empty;
    /// <summary>
    /// Gets or sets the full name associated with the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the user is an existing user.
    /// </summary>
    public bool ExistingUser { get; set; }
    /// <summary>
    /// Gets or sets the URL to which the user should be redirected after completing the current operation.
    /// </summary>
    public string? ReturnUrl { get; set; }
    /// <summary>
    /// Gets or sets the name of the provider used for the operation (Gmail,Microsoft,...).
    /// </summary>
    public string Provider { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the key for provider.
    /// </summary>
    public string ProviderKey { get; set; } = string.Empty;

}
