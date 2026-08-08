using CommentMap.Application.Features.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class LogoutModel(IMessageBus bus) : PageModel
{
    public async Task<IActionResult> OnPost(string? returnUrl = null)
    {
        await bus.InvokeAsync(new LogoutUser());
        if (returnUrl != null)
            return LocalRedirect(returnUrl);

        return RedirectToPage();
    }
}
