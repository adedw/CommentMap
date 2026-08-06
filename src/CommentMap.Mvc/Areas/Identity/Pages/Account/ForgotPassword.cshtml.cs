using System.ComponentModel.DataAnnotations;
using CommentMap.Application.Features.Identity;
using CommentMap.Shared.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ForgotPasswordModel(IMessageBus bus) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await bus.InvokeAsync<ForgotPasswordResult>(new ForgotPassword(Input.Email), ct);
        if (result.UserFound)
        {
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = result.EncodedResetCode, userId = result.UserId },
                protocol: Request.Scheme)!;

            await bus.PublishAsync(new SendResetPasswordEmail(Input.Email, callbackUrl));
        }

        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
