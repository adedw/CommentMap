using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

[Authorize]
public class ShowRecoveryCodesModel : PageModel
{
    [TempData]
    public string[] RecoveryCodes { get; set; }

    public IActionResult OnGet()
    {
        if (RecoveryCodes == null || RecoveryCodes.Length == 0)
        {
            return RedirectToPage("./TwoFactorAuthentication");
        }

        return Page();
    }
}
