using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Features.Identity.Queries;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class EnableAuthenticatorModel(IQueryHandler<GetAuthenticatorSetupQuery, AuthenticatorSetupDto?> getAuthenticatorSetupQueryHandler, ICommandHandler<EnableAuthenticatorCommand, EnableAuthenticatorResultDto> enableAuthenticatorCommandHandler) : PageModel
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

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var setup = await getAuthenticatorSetupQueryHandler.Handle(new GetAuthenticatorSetupQuery(userId), ct);
        if (setup is null)
            return NotFound($"Unable to load user with ID '{userId}'.");

        ApplySetup(setup);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();

        if (!ModelState.IsValid)
        {
            var setup = await getAuthenticatorSetupQueryHandler.Handle(new GetAuthenticatorSetupQuery(userId), ct);
            if (setup is null)
                return NotFound($"Unable to load user with ID '{userId}'.");
            ApplySetup(setup);
            return Page();
        }

        var result = await enableAuthenticatorCommandHandler.Handle(
            new EnableAuthenticatorCommand(userId, Input.Code), ct);

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
