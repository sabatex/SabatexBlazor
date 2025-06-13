using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Sabatex.Core;
using Sabatex.Core.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace Sabatex.RadzenBlazor.Identity.Account.Pages;

public partial class Register
{

    [Inject]
    private ILogger<Register> Logger { get; set; } = default!;
    
    [Inject]
    private IIdentityAdapter IdentityAdapter { get; set; } = default!;
    
    [Inject]
    private IStringLocalizer<Login> Localizer { get; set; } = default!;
    
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; } = new();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string? Message;
    

    public async Task RegisterUser(EditContext editContext)
    {
        Message = await IdentityAdapter.RegisterUserAsync(Input.Email, Input.Password, Input.FullName, ReturnUrl);
    }

    private sealed class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";
        
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = "";

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
