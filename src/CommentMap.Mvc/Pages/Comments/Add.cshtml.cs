using System.Globalization;

using CommentMap.Application.Features.Comments;
using CommentMap.Mvc.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class AddModel(IMessageBus bus) : PageModel
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
        await bus.InvokeAsync(new AddComment(
            userId,
            Input.Title!,
            Input.Text!,
            Input.Longitude!.Value,
            Input.Latitude!.Value), cancellationToken);

        return RedirectToPage("/Comments/Index");
    }

    public void OnGet()
    {
        CurrentLocale = CultureInfo.CurrentCulture.Name;
    }
}
