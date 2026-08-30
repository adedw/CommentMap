using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterConfirmationModel() : PageModel
{
    public void OnGet()
    {
    }
}
