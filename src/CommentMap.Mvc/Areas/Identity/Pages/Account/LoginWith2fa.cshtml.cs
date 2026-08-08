using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class LoginWith2faModel(IMessageBus bus) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; } = null!;

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }

    public Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        RememberMe = rememberMe;
        return Task.FromResult<IActionResult>(Page());
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return Page();

        returnUrl ??= Url.Content("~/");

        var result = await bus.InvokeAsync<LoginResultDto>(
            new LoginWith2fa(Input.TwoFactorCode, rememberMe, Input.RememberMachine));

        if (result.Succeeded)
            return LocalRedirect(returnUrl);
        if (result.IsLockedOut)
            return RedirectToPage("./Lockout");

        ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
        return Page();
    }
}
