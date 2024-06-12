using CommentMap.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class AddModel : PageModel
{
    [BindProperty]
    public required AddNewCommentInput Input { get; init; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        return RedirectToPage("/Comments/Index");
    }

    public void OnGet()
    {
    }
}
