using CommentMap.Application.Features.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wolverine;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class ConfirmDeleteModel(IMessageBus bus) : PageModel
{
    public string? Title { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SelectedOrder { get; set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        Title = await bus.InvokeAsync<string?>(new GetCommentTitle(Id), cancellationToken);
        return Page();
    }

    public async Task<RedirectToPageResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await bus.InvokeAsync(new DeleteComment(Id), cancellationToken);
        return RedirectToPage("/Comments/Index", new { SelectedOrder });
    }
}
