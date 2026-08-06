using System.ComponentModel.DataAnnotations;
using CommentMap.Application.Entities;
using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class LoginModel(IMessageBus bus, SignInManager<User> signInManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;

    public string ReturnUrl { get; set; } = null!;

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);

        returnUrl ??= Url.Content("~/");
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid)
            return Page();

        var result = await bus.InvokeAsync<LoginResultDto>(
            new LoginUser(Input.Email, Input.Password, Input.RememberMe));

        if (result.Succeeded)
            return LocalRedirect(returnUrl);
        if (result.RequiresTwoFactor)
            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
        if (result.IsLockedOut)
            return RedirectToPage("./Lockout");

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}
