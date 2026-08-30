using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ConfirmEmailChangeModel(ICommandHandler<ConfirmEmailChangeCommand, IdentityResultDto> confirmEmailChangeCommandHandler) : PageModel
{
    public async Task<IActionResult> OnGetAsync(string userId, string email, string code, CancellationToken ct = default)
    {
        if (userId == null || email == null || code == null)
            return RedirectToPage("/Index");

        var result = await confirmEmailChangeCommandHandler.Handle(new ConfirmEmailChangeCommand(userId, email, code), ct);
        if (result.Failure == IdentityFailure.UserNotFound)
            return NotFound($"Unable to load user with ID '{userId}'.");

        if (!result.Succeeded)
        {
            TempData.SetStatus(StatusMessage.Error("Error changing email."));
            return Page();
        }

        TempData.SetStatus(StatusMessage.Success("Thank you for confirming your email change."));
        return Page();
    }
}
