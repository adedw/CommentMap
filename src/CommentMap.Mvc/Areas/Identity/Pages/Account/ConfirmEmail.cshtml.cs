using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ConfirmEmailModel(IMessageBus bus) : PageModel
{
    public async Task<IActionResult> OnGetAsync(string userId, string code)
    {
        if (userId == null || code == null)
            return RedirectToPage("/Index");

        var result = await bus.InvokeAsync<IdentityResultDto>(new ConfirmEmail(userId, code));
        if (result.Errors.Any(e => e.Code == "UserNotFound"))
            return NotFound($"Unable to load user with ID '{userId}'.");

        if (result.Succeeded)
            TempData.SetStatus(StatusMessage.Success("Thank you for confirming your email."));
        else
            TempData.SetStatus(StatusMessage.Error("Error confirming your email."));
        return Page();
    }
}
