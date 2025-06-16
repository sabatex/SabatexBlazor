using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Sabatex.Core.LocalizerHelper;

namespace Sabatex.Core.Identity;

/// <summary>
/// Provides extension methods for working with claims collections.
/// </summary>
/// <remarks>This class includes methods to retrieve specific claim values from a collection of claims.</remarks>
public static class ClaimExtensions
{
    /// <summary>
    /// Retrieves the full name from a collection of claims, or a default localized message if the full name is not
    /// found.
    /// </summary>
    /// <param name="claims">The collection of claims to search for the full name.</param>
    /// <returns>The value of the claim with the type <see cref="CustomClaimTypes.FullName"/>,  or a localized default message if
    /// the claim is not present.</returns>
    public static string GetFullNameOrDefault(this IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FullName)?.Value ?? Localize<IIdentityAdapter>("Name not specified !!!");
    }
    /// <summary>
    /// Retrieves the full name from a collection of claims or returns a default localized message if the full name is
    /// not found.
    /// </summary>
    /// <param name="claims">A collection of claims, where each claim is represented as a tuple containing a type and a value.</param>
    /// <returns>The value of the claim with the type <see cref="CustomClaimTypes.FullName"/> if it exists;  otherwise, a
    /// localized string indicating that the name is not specified.</returns>
    public static string GetFullNameOrDefault(this IEnumerable<(string Type, string Value)> claims)
    {

        return claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FullName).Value ?? Localize<IIdentityAdapter>("Name not specified !!!");
    }

}