using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.Services;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class IndexModel(IListCommentsService listCommentsService, IDeleteCommentService deleteCommentService) : PageModel
{
    public List<CommentCardViewModel>? Comments { get; private set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindUserId();
        Comments = await listCommentsService.GetAllUserComments(userId, cancellationToken);
        return Page();
    }

    public async Task<RedirectToPageResult> OnPostDeleteCommentAsync(Guid id, CancellationToken cancellationToken)
    {
        await deleteCommentService.DeleteCommentAsync(id, cancellationToken);
        return RedirectToPage("/Comments/Index");
    }
}
