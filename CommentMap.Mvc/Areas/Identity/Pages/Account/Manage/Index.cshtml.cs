using CommentMap.Mvc.Data.Entities;
using CommentMap.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class IndexModel(UserManager<User> userManager, ISendEndpointProvider sendEndpointProvider)
    : PageModel
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
        [Display(Name = "New email")]
        public string? NewEmail { get; set; }
    }

    private void Load(User user)
    {
        Email = user.Email;

        Input = new InputModel
        {
            NewEmail = user.Email,
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        Load(user);
        return Page();
    }

    public async Task<ActionResult> OnPostAsync(CancellationToken ct)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            Load(user);
            return Page();
        }

        var email = await userManager.GetEmailAsync(user);
        if (Input.NewEmail == email)
        {
            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        var userId = user.Id;
        var code = await userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmailChange",
            pageHandler: null,
            values: new { area = "Identity", userId, email = Input.NewEmail, code },
        protocol: Request.Scheme);

        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:" + nameof(SendChangeEmail)));
        await endpoint.Send(new SendChangeEmail(Input.NewEmail, callbackUrl), ct);

        StatusMessage = "Confirmation link to change email sent. Please check your email.";
        return RedirectToPage();
    }
}
