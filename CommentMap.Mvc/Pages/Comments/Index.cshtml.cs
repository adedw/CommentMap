using CommentMap.Mvc.Services;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class IndexModel(IListCommentsService listCommentsService) : PageModel
{
    public List<CommentCardViewModel>? Comments { get; private set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userGuid = Guid.ParseExact(userId, "D");
        Comments = await listCommentsService.GetAllUserComments(userGuid, cancellationToken);
        return Page();
    }
}
