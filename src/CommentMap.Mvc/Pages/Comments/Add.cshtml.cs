using System.Globalization;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Comments.Commands;
using CommentMap.Mvc.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class AddModel(ICommandHandler<AddCommentCommand> addCommentCommandHandler) : PageModel
{
    [BindProperty]
    public required AddNewCommentInput Input { get; init; }

    public string? CurrentLocale { get; private set; }

    public async Task<ActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = User.FindUserId();
        await addCommentCommandHandler.Handle(new AddCommentCommand(
            userId,
            Input.Title!,
            Input.Text!,
            Input.Longitude!.Value,
            Input.Latitude!.Value), cancellationToken);

        return RedirectToPage("/Comments/Index");
    }

    public void OnGet(CancellationToken ct)
    {
        CurrentLocale = CultureInfo.CurrentCulture.Name;
    }
}
