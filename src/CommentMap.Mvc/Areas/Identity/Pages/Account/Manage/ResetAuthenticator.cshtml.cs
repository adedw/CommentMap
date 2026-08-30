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
public class ResetAuthenticatorModel(IQueryHandler<GetTwoFactorStatusQuery, TwoFactorStatusDto> getTwoFactorStatusQueryHandler, ICommandHandler<ResetAuthenticatorCommand, bool> resetAuthenticatorCommandHandler) : PageModel
{
    public async Task<IActionResult> OnGet(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var status = await getTwoFactorStatusQueryHandler.Handle(new GetTwoFactorStatusQuery(userId), ct);
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var ok = await resetAuthenticatorCommandHandler.Handle(new ResetAuthenticatorCommand(userId), ct);
        if (!ok)
            return NotFound($"Unable to load user with ID '{userId}'.");

        TempData.SetStatus(StatusMessage.Success("Your authenticator app key has been reset, you will need to configure your authenticator app using the new key."));
        return RedirectToPage("./EnableAuthenticator");
    }
}
