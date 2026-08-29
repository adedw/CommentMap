using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.EventBus.Abstractions;
using CommentMap.Shared.Messages;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class RegisterModel(ICommandHandler<RegisterUserCommand, RegisterUserResult> registerUserCommandHandler, ICommandHandler<SignInAfterRegistrationCommand> signInAfterRegistrationCommandHandler, IEventBus eventBus, SignInManager<User> signInManager) : PageModel
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

    public async Task OnGetAsync(CancellationToken ct)
    {
        ExternalLogins = [.. await signInManager.GetExternalAuthenticationSchemesAsync()];
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        ReturnUrl ??= Url.Content("~/");
        ExternalLogins = [.. await signInManager.GetExternalAuthenticationSchemesAsync()];
        if (!ModelState.IsValid)
            return Page();

        var result = await registerUserCommandHandler.Handle(
            new RegisterUserCommand(Input.Email, Input.Password), ct);

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

        await eventBus.PublishAsync(new SendConfirmEmail(Input.Email, callbackUrl));

        if (result.RequireConfirmedAccount)
            return RedirectToPage("RegisterConfirmation");

        await signInAfterRegistrationCommandHandler.Handle(new SignInAfterRegistrationCommand(result.UserId!.Value), ct);
        return LocalRedirect(ReturnUrl);
    }
}
