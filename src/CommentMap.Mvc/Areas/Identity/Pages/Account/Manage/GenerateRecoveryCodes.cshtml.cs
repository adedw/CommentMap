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
public class GenerateRecoveryCodesModel(IQueryHandler<GetTwoFactorStatusQuery, TwoFactorStatusDto> getTwoFactorStatusQueryHandler, ICommandHandler<GenerateRecoveryCodesCommand, GenerateRecoveryCodesResult> generateRecoveryCodesCommandHandler) : PageModel
{
    [TempData]
    public string[]? RecoveryCodes { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var status = await getTwoFactorStatusQueryHandler.Handle(new GetTwoFactorStatusQuery(userId), ct);
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");
        if (!status.Is2faEnabled)
            throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var result = await generateRecoveryCodesCommandHandler.Handle(new GenerateRecoveryCodesCommand(userId), ct);
        if (!result.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        RecoveryCodes = result.RecoveryCodes;
        TempData.SetStatus(StatusMessage.Success("You have generated new recovery codes."));
        return RedirectToPage("./ShowRecoveryCodes");
    }
}
