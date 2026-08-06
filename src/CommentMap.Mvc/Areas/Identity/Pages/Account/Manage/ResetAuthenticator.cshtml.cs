using CommentMap.Application.Features.Identity;
using CommentMap.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class ResetAuthenticatorModel(IMessageBus bus) : PageModel
{
    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var userId = User.FindUserId();
        var status = await bus.InvokeAsync<TwoFactorStatusDto>(new GetTwoFactorStatus(userId));
        if (!status.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindUserId();
        var ok = await bus.InvokeAsync<bool>(new ResetAuthenticator(userId));
        if (!ok)
            return NotFound($"Unable to load user with ID '{userId}'.");

        StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";
        return RedirectToPage("./EnableAuthenticator");
    }
}
