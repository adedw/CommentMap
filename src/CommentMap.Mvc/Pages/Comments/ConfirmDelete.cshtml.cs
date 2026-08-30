using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Comments.Commands;
using CommentMap.Application.Features.Comments.Queries;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class ConfirmDeleteModel(IQueryHandler<GetCommentTitleQuery, string?> getCommentTitleQueryHandler, ICommandHandler<DeleteCommentCommand> deleteCommentCommandHandler) : PageModel
{
    public string? Title { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        Title = await getCommentTitleQueryHandler.Handle(new GetCommentTitleQuery(Id), cancellationToken);
        if (Title is null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<RedirectToPageResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await deleteCommentCommandHandler.Handle(new DeleteCommentCommand(Id), cancellationToken);
        return RedirectToPage("/Comments/Index");
    }
}
