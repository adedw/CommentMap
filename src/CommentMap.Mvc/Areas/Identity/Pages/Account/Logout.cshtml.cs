using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class LogoutModel(ICommandHandler<LogoutUserCommand> logoutUserCommandHandler) : PageModel
{
    public async Task<IActionResult> OnPost(string? returnUrl = null, CancellationToken ct = default)
    {
        await logoutUserCommandHandler.Handle(new LogoutUserCommand(), ct);
        if (returnUrl != null)
            return LocalRedirect(returnUrl);

        return RedirectToPage();
    }
}
