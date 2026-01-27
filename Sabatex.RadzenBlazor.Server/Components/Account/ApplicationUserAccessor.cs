using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Sabatex.Core.Identity;
using Sabatex.RadzenBlazor.Server;


namespace Sabatex.Identity.UI;
/// <summary>
/// Provides access to the current application user and handles scenarios where the user is required but not found.
/// </summary>
/// <param name="userManager">The user manager used to retrieve application user information from the current context.</param>
/// <param name="redirectManager">The identity adapter responsible for handling redirects when user access fails.</param>
public sealed class ApplicationUserAccessor(UserManager<ApplicationUser> userManager, IIdentityAdapter redirectManager)
{
    /// <summary>
    /// Retrieves the authenticated user associated with the specified HTTP context. Throws an exception or redirects if
    /// the user cannot be found.
    /// </summary>
    /// <remarks>This method is intended for scenarios where a valid, authenticated user is required. If the
    /// user cannot be found, the method initiates a redirect to an error page and may not return normally.</remarks>
    /// <param name="context">The HTTP context containing the user principal from which to retrieve the user information.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authenticated <see
    /// cref="ApplicationUser"/> associated with the current context.</returns>
    public async Task<ApplicationUser?> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            //redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
            return null;
        }
        else
            return user;
    }
}
