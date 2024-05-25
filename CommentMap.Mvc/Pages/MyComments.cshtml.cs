using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages;

[Authorize]
public class MyCommentsModel : PageModel
{
    private readonly ILogger<MyCommentsModel> _logger;

    public MyCommentsModel(ILogger<MyCommentsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}
