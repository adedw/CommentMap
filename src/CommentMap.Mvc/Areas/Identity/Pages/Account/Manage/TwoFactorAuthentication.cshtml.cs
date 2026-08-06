using CommentMap.Application.Features.Identity;
using CommentMap.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class TwoFactorAuthenticationModel(IMessageBus bus) : PageModel
{
    public bool HasAuthenticator { get; set; }
    public int RecoveryCodesLeft { get; set; }

    [BindProperty]
    public bool Is2faEnabled { get; set; }

    public bool IsMachineRemembered { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindUserId();
        var status = await bus.InvokeAsync<TwoFactorStatusDto>(new GetTwoFactorStatus(userId));
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        HasAuthenticator = status.HasAuthenticator;
        Is2faEnabled = status.Is2faEnabled;
        IsMachineRemembered = status.IsMachineRemembered;
        RecoveryCodesLeft = status.RecoveryCodesLeft;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindUserId();
        var ok = await bus.InvokeAsync<bool>(new ForgetTwoFactorClient(userId));
        if (!ok)
            return NotFound($"Unable to load user with ID '{userId}'.");

        StatusMessage = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
        return RedirectToPage();
    }
}
