using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sabatex.Core.Identity;
using System.Security.Claims;

namespace Sabatex.RadzenBlazor.Server;

public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (!string.IsNullOrEmpty(user.FullName))
        {
            identity.AddClaim(new Claim(CustomClaimTypes.FullName, user.FullName));
        }

        return identity;
    }
}