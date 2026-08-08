using System.ComponentModel.DataAnnotations;

using CommentMap.Application.Features.Identity;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public class DeletePersonalDataModel(IMessageBus bus) : PageModel
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

    public async Task<IActionResult> OnGet()
    {
        var userId = User.FindUserId();
        var info = await bus.InvokeAsync<GetDeleteProfileInfoResult>(new GetDeleteProfileInfo(userId));
        if (!info.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        RequirePassword = info.RequirePassword;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindUserId();
        var info = await bus.InvokeAsync<GetDeleteProfileInfoResult>(new GetDeleteProfileInfo(userId));
        if (!info.Found)
            return NotFound($"Unable to load user with ID '{userId}'.");

        RequirePassword = info.RequirePassword;
        var result = await bus.InvokeAsync<IdentityResultDto>(
            new DeleteProfile(userId, RequirePassword ? Input.Password : null));

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        return Redirect("~/");
    }
}
