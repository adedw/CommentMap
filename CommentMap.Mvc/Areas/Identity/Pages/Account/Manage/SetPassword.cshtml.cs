using System.ComponentModel.DataAnnotations;
using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class SetPasswordModel(IMessageBus bus) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindUserId();
        var hasPassword = await bus.InvokeAsync<HasPasswordResult>(new HasPassword(userId));
        if (!hasPassword.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");
        if (hasPassword.HasPassword)
            return RedirectToPage("./ChangePassword");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var userId = User.FindUserId();
        var result = await bus.InvokeAsync<IdentityResultDto>(new SetPassword(userId, Input.NewPassword));

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == "UserNotFound"))
                return NotFound($"Unable to load user with ID '{userId}'.");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        StatusMessage = "Your password has been set.";
        return RedirectToPage();
    }
}
