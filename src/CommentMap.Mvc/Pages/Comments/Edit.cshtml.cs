using CommentMap.Application.Features.Comments;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class EditModel(IMessageBus bus) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public string? Title { get; set; }

    [BindProperty]
    public string? Text { get; set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        Title = await bus.InvokeAsync<string?>(new GetCommentTitle(Id), cancellationToken);
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
