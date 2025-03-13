using System.ComponentModel.DataAnnotations;
using System.Text;
using CommentMap.Mvc.Data.Entities;
using CommentMap.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

public class ForgotPasswordModel(UserManager<User> userManager, ISendEndpointProvider sendEndpointProvider)
    : PageModel
{
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

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await userManager.FindByEmailAsync(Input.Email);
        if (user == null || !await userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { area = "Identity", code, userId = user.Id },
        protocol: Request.Scheme);

        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:" + nameof(SendResetPasswordEmail)));
        await endpoint.Send(new SendResetPasswordEmail(Input.Email, callbackUrl), ct);
        
        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
