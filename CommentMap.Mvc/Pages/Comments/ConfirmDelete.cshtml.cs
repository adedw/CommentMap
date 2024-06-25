using CommentMap.Mvc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages.Comments;

public class ConfirmDeleteModel(IConfirmDeleteService confirmDeleteService, IDeleteCommentService deleteCommentService) : PageModel
{
    public string? Title { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SelectedOrder { get; set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        Title = await confirmDeleteService.GetCommentTitleAsync(Id, cancellationToken);
        return Page();
    }

    public async Task<RedirectToPageResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await deleteCommentService.DeleteCommentAsync(Id, cancellationToken);
        return RedirectToPage("/Comments/Index", new { SelectedOrder });
    }
}
