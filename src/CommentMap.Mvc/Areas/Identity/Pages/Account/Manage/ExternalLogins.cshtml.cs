using CommentMap.Application.Entities;
using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class ExternalLoginsModel(IMessageBus bus, SignInManager<User> signInManager) : PageModel
{
    public IList<UserLoginInfo> CurrentLogins { get; set; } = null!;
    public IList<AuthenticationScheme> OtherLogins { get; set; } = null!;
    public bool ShowRemoveButton { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindUserId();
        var dto = await bus.InvokeAsync<ExternalLoginsDto>(new GetExternalLogins(userId));
        if (!dto.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        CurrentLogins = dto.CurrentLogins.ToList();
        var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
        OtherLogins = schemes.Where(s => dto.OtherLoginProviderNames.Contains(s.Name!)).ToList();
        ShowRemoveButton = dto.ShowRemoveButton;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey)
    {
        var userId = User.FindUserId();
        var result = await bus.InvokeAsync<IdentityResultDto>(
            new RemoveExternalLogin(userId, loginProvider, providerKey));

        TempData.SetStatus(result.Succeeded
            ? StatusMessage.Success("The external login was removed.")
            : StatusMessage.Error("The external login was not removed."));
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLinkLoginAsync(string provider)
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider, redirectUrl, User.FindUserId().ToString());
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetLinkLoginCallbackAsync()
    {
        var userId = User.FindUserId();
        var result = await bus.InvokeAsync<IdentityResultDto>(new LinkExternalLogin(userId));
        if (result.Errors.Any(e => e.Code == "UserNotFound"))
            return NotFound($"Unable to load user with ID '{userId}'.");

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        TempData.SetStatus(result.Succeeded
            ? StatusMessage.Success("The external login was added.")
            : StatusMessage.Error("The external login was not added. External logins can only be associated with one account."));
        return RedirectToPage();
    }
}
