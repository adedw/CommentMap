using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Comments.Queries;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class EditModel(IQueryHandler<GetCommentTitleQuery, string?> getCommentTitleQueryHandler) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public string? Title { get; set; }

    [BindProperty]
    public string? Text { get; set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        Title = await getCommentTitleQueryHandler.Handle(new GetCommentTitleQuery(Id), cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: implement UpdateComment handler and invoke it here
        await Task.CompletedTask;

        return RedirectToPage("/Comments/Index");
    }
}
