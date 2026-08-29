using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Features.Identity.Queries;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ExternalLoginsModel(IQueryHandler<GetExternalLoginsQuery, ExternalLoginsDto> getExternalLoginsQueryHandler, ICommandHandler<RemoveExternalLoginCommand, IdentityResultDto> removeExternalLoginCommandHandler, ICommandHandler<LinkExternalLoginCommand, IdentityResultDto> linkExternalLoginCommandHandler, SignInManager<User> signInManager) : PageModel
{
    public IList<UserLoginInfo> CurrentLogins { get; set; } = null!;
    public IList<AuthenticationScheme> OtherLogins { get; set; } = null!;
    public bool ShowRemoveButton { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var dto = await getExternalLoginsQueryHandler.Handle(new GetExternalLoginsQuery(userId), ct);
        if (!dto.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        CurrentLogins = dto.CurrentLogins.ToList();
        var schemes = await signInManager.GetExternalAuthenticationSchemesAsync();
        OtherLogins = schemes.Where(s => dto.OtherLoginProviderNames.Contains(s.Name!)).ToList();
        ShowRemoveButton = dto.ShowRemoveButton;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveLoginAsync(string loginProvider, string providerKey, CancellationToken ct = default)
    {
        var userId = User.FindUserId();
        var result = await removeExternalLoginCommandHandler.Handle(
            new RemoveExternalLoginCommand(userId, loginProvider, providerKey), ct);

        TempData.SetStatus(result.Succeeded
            ? StatusMessage.Success("The external login was removed.")
            : StatusMessage.Error("The external login was not removed."));
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLinkLoginAsync(string provider, CancellationToken ct = default)
    {
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        var redirectUrl = Url.Page("./ExternalLogins", pageHandler: "LinkLoginCallback");
        var properties = signInManager.ConfigureExternalAuthenticationProperties(
            provider, redirectUrl, User.FindUserId().ToString());
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetLinkLoginCallbackAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var result = await linkExternalLoginCommandHandler.Handle(new LinkExternalLoginCommand(userId), ct);
        if (result.Failure == IdentityFailure.UserNotFound)
            return NotFound($"Unable to load user with ID '{userId}'.");

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        TempData.SetStatus(result.Succeeded
            ? StatusMessage.Success("The external login was added.")
            : StatusMessage.Error("The external login was not added. External logins can only be associated with one account."));
        return RedirectToPage();
    }
}
