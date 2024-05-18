using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Pages;

[Authorize]
public class PrivateModel : PageModel
{
    private readonly ILogger<PrivateModel> _logger;

    public PrivateModel(ILogger<PrivateModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}
