using System.ComponentModel.DataAnnotations;
using System.Text;
using CommentMap.Mvc.Data.Entities;
using CommentMap.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResendEmailConfirmationModel(UserManager<User> userManager, ISendEndpointProvider sendEndpointProvider)
    : PageModel
{
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
        [EmailAddress]
        public string Email { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Unable to find user by email.");
            return Page();
        }

        var userId = user.Id;
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new { userId, code },
        protocol: Request.Scheme);

        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:" + nameof(SendConfirmEmail)));
        await endpoint.Send(new SendConfirmEmail(Input.Email, callbackUrl), ct);

        StatusMessage = "Verification email sent. Please check your email.";
        return Page();
    }
}
