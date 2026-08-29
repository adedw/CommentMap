using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Queries;
using CommentMap.EventBus.Abstractions;
using CommentMap.Shared.Messages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ForgotPasswordModel(IQueryHandler<ForgotPasswordQuery, ForgotPasswordResult> forgotPasswordQueryHandler, IEventBus eventBus) : PageModel
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

        var result = await forgotPasswordQueryHandler.Handle(new ForgotPasswordQuery(Input.Email), ct);
        if (result.UserFound)
        {
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code = result.EncodedResetCode, userId = result.UserId },
                protocol: Request.Scheme)!;

            await eventBus.PublishAsync(new SendResetPasswordEmail(Input.Email, callbackUrl));
        }

        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
