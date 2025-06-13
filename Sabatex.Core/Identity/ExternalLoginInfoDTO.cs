using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core.Identity;

public class ExternalLoginInfoDTO
{
    public string Provider { get; init; } = "";
    public string ProviderKey { get; init; } = "";
    public IEnumerable<(string Type, string Value)> Claims { get; init; } = Array.Empty<(string, string)>();

    // Нові властивості для зручності
    public string? Email =>
      Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

    public string? Name => Claims.GetFullNameOrDefault();
}
