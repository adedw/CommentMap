using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class LoginWithRecoveryCodeModel(ICommandHandler<LoginWithRecoveryCodeCommand, LoginResultDto> loginWithRecoveryCodeCommandHandler) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; } = null!;
    }

    public Task<IActionResult> OnGetAsync(string? returnUrl = null, CancellationToken ct = default)
    {
        ReturnUrl = returnUrl;
        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return Page();

        returnUrl ??= Url.Content("~/");

        var result = await loginWithRecoveryCodeCommandHandler.Handle(new LoginWithRecoveryCodeCommand(Input.RecoveryCode), ct);

        if (result.Succeeded)
            return LocalRedirect(returnUrl);
        if (result.IsLockedOut)
            return RedirectToPage("./Lockout");

        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
        return Page();
    }
}
