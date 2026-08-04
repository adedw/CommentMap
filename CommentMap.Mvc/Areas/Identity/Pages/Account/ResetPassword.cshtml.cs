using System.ComponentModel.DataAnnotations;
using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ResetPasswordModel(IMessageBus bus) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? UserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Code { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(otherProperty: nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public IActionResult OnGet(string? code = null)
    {
        if (code is null)
            return BadRequest("A code must be supplied for password reset.");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await bus.InvokeAsync<IdentityResultDto>(
            new ResetPassword(UserId!, Code!, Input.Password));

        if (result.Succeeded)
            return RedirectToPage("./ResetPasswordConfirmation");

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }
}
