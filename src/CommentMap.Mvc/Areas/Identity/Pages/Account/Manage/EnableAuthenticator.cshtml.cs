using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class EnableAuthenticatorModel(IMessageBus bus) : PageModel
{
    public string SharedKey { get; set; } = null!;
    public string AuthenticatorUri { get; set; } = null!;
    public string? QrCode { get; set; }

    [TempData]
    public string[]? RecoveryCodes { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; } = null!;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindUserId();
        var setup = await bus.InvokeAsync<AuthenticatorSetupDto?>(new GetAuthenticatorSetup(userId));
        if (setup is null)
            return NotFound($"Unable to load user with ID '{userId}'.");

        ApplySetup(setup);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindUserId();

        if (!ModelState.IsValid)
        {
            var setup = await bus.InvokeAsync<AuthenticatorSetupDto?>(new GetAuthenticatorSetup(userId));
            if (setup is null)
                return NotFound($"Unable to load user with ID '{userId}'.");
            ApplySetup(setup);
            return Page();
        }

        var result = await bus.InvokeAsync<EnableAuthenticatorResultDto>(
            new EnableAuthenticator(userId, Input.Code));

        if (result is { Succeeded: false, Setup: null })
            return NotFound($"Unable to load user with ID '{userId}'.");

        if (result.InvalidCode)
        {
            ModelState.AddModelError("Input.Code", "Verification code is invalid.");
            ApplySetup(result.Setup!);
            return Page();
        }

        TempData.SetStatus(StatusMessage.Success("Your authenticator app has been verified."));

        if (result.ShowRecoveryCodes)
        {
            RecoveryCodes = result.RecoveryCodes;
            return RedirectToPage("./ShowRecoveryCodes");
        }

        return RedirectToPage("./TwoFactorAuthentication");
    }

    private void ApplySetup(AuthenticatorSetupDto setup)
    {
        SharedKey = setup.SharedKey;
        AuthenticatorUri = setup.AuthenticatorUri;
        QrCode = setup.QrCodeEmbedded;
    }
}
