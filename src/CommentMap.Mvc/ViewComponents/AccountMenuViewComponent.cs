using CommentMap.Application.Entities;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommentMap.Mvc.ViewComponents;

/// <summary>
/// Renders the account menu in the navigation bar. Shows Account and Logout
/// links for signed-in users, or Register and Login links for guests.
/// </summary>
public class AccountMenuViewComponent(SignInManager<User> signInManager) : ViewComponent
{
    /// <summary>
    /// Builds the menu view model from the current user's sign-in state.
    /// </summary>
    public IViewComponentResult Invoke()
    {
        var isSignedIn = signInManager.IsSignedIn(UserClaimsPrincipal);
        return View(new AccountMenuViewModel(isSignedIn));
    }
}
