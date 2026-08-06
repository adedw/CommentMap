using CommentMap.Application.Features.Comments;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using Wolverine;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class AddModel(IMessageBus bus) : PageModel
{
    [BindProperty]
    public required AddNewCommentInput Input { get; init; }

    [BindProperty(SupportsGet = true)]
    public int SelectedOrder { get; set; }

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
            Input.Location.Longitude!.Value,
            Input.Location.Latitude!.Value), cancellationToken);

        return RedirectToPage("/Comments/Index", new { SelectedOrder });
    }

    public void OnGet()
    {
        CurrentLocale = CultureInfo.CurrentCulture.Name;
    }
}
