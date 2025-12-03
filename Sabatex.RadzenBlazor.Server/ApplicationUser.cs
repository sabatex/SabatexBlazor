using Microsoft.AspNetCore.Identity;

namespace Sabatex.RadzenBlazor.Server;

/// <summary>
/// extend IdentityUser for FullName
/// </summary>
public class ApplicationUser: IdentityUser
{
    /// <summary>
    /// Gets or sets the full name of the user identity.
    /// </summary>
    public string? FullName { get; set; }
}
