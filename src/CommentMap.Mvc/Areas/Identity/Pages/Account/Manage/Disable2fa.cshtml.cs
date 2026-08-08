using CommentMap.Application.Features.Identity;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class Disable2faModel(IMessageBus bus) : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        var userId = User.FindUserId();
        var status = await bus.InvokeAsync<TwoFactorStatusDto>(new GetTwoFactorStatus(userId));
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");
        if (!status.Is2faEnabled)
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindUserId();
        var ok = await bus.InvokeAsync<bool>(new Disable2fa(userId));
        if (!ok)
            return NotFound($"Unable to load user with ID '{userId}'.");

        TempData.SetStatus(StatusMessage.Success("2fa has been disabled. You can reenable 2fa when you setup an authenticator app"));
        return RedirectToPage("./TwoFactorAuthentication");
    }
}
