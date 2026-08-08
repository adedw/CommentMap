using CommentMap.Application.Features.Identity;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class GenerateRecoveryCodesModel(IMessageBus bus) : PageModel
{
    [TempData]
    public string[]? RecoveryCodes { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindUserId();
        var status = await bus.InvokeAsync<TwoFactorStatusDto>(new GetTwoFactorStatus(userId));
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");
        if (!status.Is2faEnabled)
            throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindUserId();
        var result = await bus.InvokeAsync<GenerateRecoveryCodesResult>(new GenerateRecoveryCodes(userId));
        if (!result.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        RecoveryCodes = result.RecoveryCodes;
        TempData.SetStatus(StatusMessage.Success("You have generated new recovery codes."));
        return RedirectToPage("./ShowRecoveryCodes");
    }
}
