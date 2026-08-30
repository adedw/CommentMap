using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity.Commands;
using CommentMap.Application.Features.Identity.Queries;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class DeletePersonalDataModel(IQueryHandler<GetDeleteProfileInfoQuery, GetDeleteProfileInfoResult> getDeleteProfileInfoQueryHandler, ICommandHandler<DeleteProfileCommand, IdentityResultDto> deleteProfileCommandHandler) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGet(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var info = await getDeleteProfileInfoQueryHandler.Handle(new GetDeleteProfileInfoQuery(userId), ct);
        if (!info.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        RequirePassword = info.RequirePassword;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var userId = User.FindUserId();
        var info = await getDeleteProfileInfoQueryHandler.Handle(new GetDeleteProfileInfoQuery(userId), ct);
        if (!info.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        RequirePassword = info.RequirePassword;
        var result = await deleteProfileCommandHandler.Handle(
            new DeleteProfileCommand(userId, RequirePassword ? Input.Password : null), ct);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        return Redirect("~/");
    }
}
