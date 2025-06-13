using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.Identity;

public record ExtermnalRegisterDTO
{
    public string Email { get; set; }
    public string Name { get; set; }
    public bool ExistingUser { get; set; }
    public string? ReturnUrl { get; set; }
    public string Provider { get; set; }
    public string ProviderKey { get; set; }

}
