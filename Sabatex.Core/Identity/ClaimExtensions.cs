using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Sabatex.Core.LocalizerHelper;

namespace Sabatex.Core.Identity;

public static class ClaimExtensions
{
    public static string GetFullNameOrDefault(this IEnumerable<Claim> claims)
    {

        return claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FullName)?.Value ?? Localize<IIdentityAdapter>("Name not specified !!!");
    }
    public static string GetFullNameOrDefault(this IEnumerable<(string Type, string Value)> claims)
    {

        return claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FullName).Value ?? Localize<IIdentityAdapter>("Name not specified !!!");
    }

}