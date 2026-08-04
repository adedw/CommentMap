using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ConfirmEmailChangeModel(IMessageBus bus) : PageModel
{
    [TempData]
    public string StatusMessage { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
    {
        if (userId == null || email == null || code == null)
            return RedirectToPage("/Index");

        var result = await bus.InvokeAsync<IdentityResultDto>(new ConfirmEmailChange(userId, email, code));
        if (result.Errors.Any(e => e.Code == "UserNotFound"))
            return NotFound($"Unable to load user with ID '{userId}'.");

        if (!result.Succeeded)
        {
            StatusMessage = "Error changing email.";
            return Page();
        }

        StatusMessage = "Thank you for confirming your email change.";
        return Page();
    }
}
