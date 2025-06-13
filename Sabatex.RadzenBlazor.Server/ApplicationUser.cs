using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Server;

/// <summary>
/// extend IdentityUser for Name
/// </summary>
public class ApplicationUser: IdentityUser
{
    public string? Name { get; set; }
}
