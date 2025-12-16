using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Sabatex.Core.Identity;
using Sabatex.RadzenBlazor.Server;


namespace Sabatex.Identity.UI;

public sealed class ApplicationUserAccessor(UserManager<ApplicationUser> userManager, IIdentityAdapter redirectManager)
{
    public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            //redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.");

        }

        return user;
    }
}
