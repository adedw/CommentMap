using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Features.Identity.Queries;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class Disable2FAModel(IQueryHandler<GetTwoFactorStatusQuery, TwoFactorStatusDto> getTwoFactorStatusQueryHandler, ICommandHandler<Disable2FACommand, bool> disable2faCommandHandler) : PageModel
{
    public async Task<IActionResult> OnGet(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var status = await getTwoFactorStatusQueryHandler.Handle(new GetTwoFactorStatusQuery(userId), ct);
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");
        if (!status.Is2faEnabled)
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var ok = await disable2faCommandHandler.Handle(new Disable2FACommand(userId), ct);
        if (!ok)
            return NotFound($"Unable to load user with ID '{userId}'.");

        TempData.SetStatus(StatusMessage.Success("2fa has been disabled. You can reenable 2fa when you setup an authenticator app"));
        return RedirectToPage("./TwoFactorAuthentication");
    }
}
