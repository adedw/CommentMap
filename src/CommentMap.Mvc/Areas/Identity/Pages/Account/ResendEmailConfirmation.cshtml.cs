using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Features.Identity;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;
using CommentMap.Shared.Messages;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResendEmailConfirmationModel(IMessageBus bus) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await bus.InvokeAsync<ResendEmailConfirmationResult>(
            new ResendEmailConfirmation(Input.Email), ct);

        if (!result.UserFound)
        {
            ModelState.AddModelError(string.Empty, "Unable to find user by email.");
            return Page();
        }

        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { userId = result.UserId, code = result.EncodedCode },
            protocol: Request.Scheme)!;

        await bus.PublishAsync(new SendConfirmEmail(Input.Email, callbackUrl));

        TempData.SetStatus(StatusMessage.Success("Verification email sent. Please check your email."));
        return Page();
    }
}
