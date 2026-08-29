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
public class ChangePasswordModel(IQueryHandler<HasPasswordQuery, HasPasswordResult> hasPasswordQueryHandler, ICommandHandler<ChangePasswordCommand, IdentityResultDto> changePasswordCommandHandler) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; } = null!;

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
        if (!hasPassword.HasPassword)
            return RedirectToPage("./SetPassword");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Page();

        var userId = User.FindUserId();
        var result = await changePasswordCommandHandler.Handle(
            new ChangePasswordCommand(userId, Input.OldPassword, Input.NewPassword), ct);

        if (!result.Succeeded)
        {
            if (result.Failure == IdentityFailure.UserNotFound)
                return NotFound($"Unable to load user with ID '{userId}'.");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        TempData.SetStatus(StatusMessage.Success("Your password has been changed."));
        return RedirectToPage();
    }
}
