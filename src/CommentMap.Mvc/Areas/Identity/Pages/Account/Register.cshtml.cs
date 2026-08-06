using CommentMap.Application.Entities;
using CommentMap.Application.Features.Identity;
using CommentMap.Shared.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class RegisterModel(IMessageBus bus, SignInManager<User> signInManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public async Task OnGetAsync()
    {
        ExternalLogins = [.. await signInManager.GetExternalAuthenticationSchemesAsync()];
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        ReturnUrl ??= Url.Content("~/");
        ExternalLogins = [.. await signInManager.GetExternalAuthenticationSchemesAsync()];
        if (!ModelState.IsValid)
            return Page();

        var result = await bus.InvokeAsync<RegisterUserResult>(
            new RegisterUser(Input.Email, Input.Password), ct);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { area = "Identity", userId = result.UserId, code = result.EncodedEmailConfirmationCode, ReturnUrl },
            protocol: Request.Scheme)!;

        await bus.PublishAsync(new SendConfirmEmail(Input.Email, callbackUrl));

        if (result.RequireConfirmedAccount)
            return RedirectToPage("RegisterConfirmation");

        await bus.InvokeAsync(new SignInAfterRegistration(result.UserId!.Value), ct);
        return LocalRedirect(ReturnUrl);
    }
}
