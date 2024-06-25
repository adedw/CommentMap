using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.Models;
using CommentMap.Mvc.Services;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class IndexModel(IListCommentsService listCommentsService, IHttpContextAccessor httpContextAccessor) : PageModel
{
    public List<CommentCardViewModel>? Comments { get; private set; }

    [BindProperty(SupportsGet = true)]
    public Order SelectedOrder { get; set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindUserId();
        var dto = new GetAllCommentsDto(userId, SelectedOrder);
        Comments = await listCommentsService.GetAllUserComments(dto, cancellationToken);
        return Page();
    }
}
