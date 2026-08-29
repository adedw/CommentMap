using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ConfirmEmailModel(ICommandHandler<ConfirmEmailCommand, IdentityResultDto> confirmEmailCommandHandler) : PageModel
{
    public async Task<IActionResult> OnGetAsync(string userId, string code, CancellationToken ct = default)
    {
        if (userId == null || code == null)
            return RedirectToPage("/Index");

        var result = await confirmEmailCommandHandler.Handle(new ConfirmEmailCommand(userId, code), ct);
        if (result.Failure == IdentityFailure.UserNotFound)
            return NotFound($"Unable to load user with ID '{userId}'.");

        if (result.Succeeded)
            TempData.SetStatus(StatusMessage.Success("Thank you for confirming your email."));
        else
            TempData.SetStatus(StatusMessage.Error("Error confirming your email."));
        return Page();
    }
}
