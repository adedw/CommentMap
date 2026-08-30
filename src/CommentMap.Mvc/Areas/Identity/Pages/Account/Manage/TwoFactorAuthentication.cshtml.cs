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
public class TwoFactorAuthenticationModel(IQueryHandler<GetTwoFactorStatusQuery, TwoFactorStatusDto> getTwoFactorStatusQueryHandler, ICommandHandler<ForgetTwoFactorClientCommand, bool> forgetTwoFactorClientCommandHandler) : PageModel
{
    public bool HasAuthenticator { get; set; }
    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var status = await getTwoFactorStatusQueryHandler.Handle(new GetTwoFactorStatusQuery(userId), ct);
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        HasAuthenticator = status.HasAuthenticator;
        Is2faEnabled = status.Is2faEnabled;
        IsMachineRemembered = status.IsMachineRemembered;
        RecoveryCodesLeft = status.RecoveryCodesLeft;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var ok = await forgetTwoFactorClientCommandHandler.Handle(new ForgetTwoFactorClientCommand(userId), ct);
        if (!ok)
            return NotFound($"Unable to load user with ID '{userId}'.");

        TempData.SetStatus(StatusMessage.Success("The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code."));
        return RedirectToPage();
    }
}
