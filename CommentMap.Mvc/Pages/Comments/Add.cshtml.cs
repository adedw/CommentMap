using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.Models;
using CommentMap.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class AddModel(IAddCommentService addCommentService) : PageModel
{
    [BindProperty]
    public required AddNewCommentInput Input { get; init; }

    [BindProperty(SupportsGet = true)]
    public int SelectedOrder { get; set; }

    public async Task<ActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = User.FindUserId();
        var addCommentDto = new AddNewCommentDto(userId, Input.Title!, Input.Text!, Input.Location.Longitude!.Value, Input.Location.Latitude!.Value);

        await addCommentService.AddAsync(addCommentDto, cancellationToken);

        return RedirectToPage("/Comments/Index", new { SelectedOrder });
    }

    public void OnGet()
    {
    }
}
