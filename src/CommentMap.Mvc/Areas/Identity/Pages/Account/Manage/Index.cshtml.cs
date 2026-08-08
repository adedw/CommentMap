using CommentMap.Application.Features.Identity;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;
using CommentMap.Shared.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class IndexModel(IMessageBus bus) : PageModel
{
    public string? Email { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [Display(Name = "New email")]
        public string? NewEmail { get; set; }
    }

    private void Load(string? email)
    {
        Email = email;
        Input = new InputModel { NewEmail = email };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindUserId();
        var profile = await bus.InvokeAsync<GetProfileEmailResult>(new GetProfileEmail(userId));
        if (!profile.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        Load(profile.Email);
        return Page();
    }

    public async Task<ActionResult> OnPostAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var profile = await bus.InvokeAsync<GetProfileEmailResult>(new GetProfileEmail(userId), ct);
        if (!profile.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        if (!ModelState.IsValid)
        {
            Load(profile.Email);
            return Page();
        }

        var result = await bus.InvokeAsync<RequestEmailChangeResult>(
            new RequestEmailChange(userId, Input.NewEmail!), ct);

        if (result.Unchanged)
        {
            TempData.SetStatus(StatusMessage.Info("Your email is unchanged."));
            return RedirectToPage();
        }

        var callbackUrl = Url.Page(
            "/Account/ConfirmEmailChange",
            pageHandler: null,
            values: new { area = "Identity", userId, email = Input.NewEmail, code = result.EncodedCode },
            protocol: Request.Scheme)!;

        await bus.PublishAsync(new SendChangeEmail(Input.NewEmail!, callbackUrl));

        TempData.SetStatus(StatusMessage.Info("Confirmation link to change email sent. Please check your email."));
        return RedirectToPage();
    }
}
