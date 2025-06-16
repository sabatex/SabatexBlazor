using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
