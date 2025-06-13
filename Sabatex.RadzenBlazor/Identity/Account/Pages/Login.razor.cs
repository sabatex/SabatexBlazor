using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sabatex.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Identity.Account.Pages;

public partial class Login
{
    private string? errorMessage;


    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [Inject]
    private ILogger<Login> Logger { get; set; } = default!;
    [Inject]
    private IIdentityAdapter IdentityAdapter { get; set; } = default!;
    [Inject]
    private IStringLocalizer<Login> Localizer { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // if (HttpMethods.IsGet(HttpContext.Request.Method))
        // {
        //     // Clear the existing external cookie to ensure a clean login process
        //     await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        // }
    }

    public async Task LoginUser()
    {
        Logger.LogInformation("Attempting login for user {Email}", Input.Email);
        var status = await IdentityAdapter.SignInAsync(Input.Email, Input.Password, Input.RememberMe);

        switch (status)
        {
            case SignInStatus.Success:
                Logger.LogInformation("User {Email} successfully logged in.", Input.Email);
                IdentityAdapter.RedirectTo(ReturnUrl ?? "/");
                break;

            case SignInStatus.RequiresTwoFactor:
                Logger.LogInformation("User {Email} requires two-factor authentication.", Input.Email);
                IdentityAdapter.RedirectTo("Account/LoginWith2fa", new Dictionary<string, object?>
                {
                    ["returnUrl"] = ReturnUrl,
                    ["rememberMe"] = Input.RememberMe
                });
                break;

            case SignInStatus.LockedOut:
                Logger.LogWarning("User {Email} is locked out.", Input.Email);
                IdentityAdapter.RedirectTo("Account/Lockout");
                break;

            case SignInStatus.InvalidCredentials:
                Logger.LogWarning("Invalid login attempt for user {Email}.", Input.Email);
                errorMessage = "Error: " + Localizer["Invalid login attempt for user {0}.", Input.Email];
                break;


        }
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

}
