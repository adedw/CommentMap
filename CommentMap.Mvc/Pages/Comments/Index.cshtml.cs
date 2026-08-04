using CommentMap.Application.Features.Comments;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class IndexModel(IMessageBus bus) : PageModel
{
    public List<CommentCardViewModel>? Comments { get; private set; }

    [BindProperty(SupportsGet = true)]
    public Order SelectedOrder { get; set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindUserId();
        var items = await bus.InvokeAsync<List<CommentCardDto>>(
            new ListComments(userId, SelectedOrder), cancellationToken);

        Comments = [.. items.Select(c => new CommentCardViewModel(
            c.Id,
            new LocationViewModel { Longitude = c.Longitude, Latitude = c.Latitude },
            c.Title,
            c.Text,
            c.CreatedAt))];

        return Page();
    }
}
