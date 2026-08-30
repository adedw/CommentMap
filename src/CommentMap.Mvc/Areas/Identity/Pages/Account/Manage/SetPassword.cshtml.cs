using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Features.Identity.Queries;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class SetPasswordModel(IQueryHandler<HasPasswordQuery, HasPasswordResult> hasPasswordQueryHandler, ICommandHandler<SetPasswordCommand, IdentityResultDto> setPasswordCommandHandler) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

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

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var hasPassword = await hasPasswordQueryHandler.Handle(new HasPasswordQuery(userId), ct);
        if (!hasPassword.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");
        if (hasPassword.HasPassword)
            return RedirectToPage("./ChangePassword");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Page();

        var userId = User.FindUserId();
        var result = await setPasswordCommandHandler.Handle(new SetPasswordCommand(userId, Input.NewPassword), ct);

        if (!result.Succeeded)
        {
            if (result.Failure == IdentityFailure.UserNotFound)
                return NotFound($"Unable to load user with ID '{userId}'.");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        TempData.SetStatus(StatusMessage.Success("Your password has been set."));
        return RedirectToPage();
    }
}
