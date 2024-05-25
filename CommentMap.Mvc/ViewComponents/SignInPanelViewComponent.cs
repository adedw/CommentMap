using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommentMap.Mvc.ViewComponents;

public class SignInPanelViewComponent(SignInManager<User> signInManager) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var isSignedIn = signInManager.IsSignedIn(UserClaimsPrincipal);
        return View(new SignInPanelViewModel(isSignedIn));
    }
}
