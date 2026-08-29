using Microsoft.AspNetCore.Mvc.Rendering;

namespace CommentMap.Mvc.Areas.Identity.Pages.Account.Manage;

public static class ManageNavPages
{
    public const string Index = nameof(Index);
    public const string ChangePassword = nameof(ChangePassword);
    public const string ExternalLogins = nameof(ExternalLogins);
    public const string TwoFactorAuthentication = nameof(TwoFactorAuthentication);
    public const string DeleteProfile = nameof(DeleteProfile);

    public static string ActivePageKey => "ActivePage";

    public static string NavClass(ViewContext viewContext, string page) 
        => Equals(viewContext.ViewData[ActivePageKey], page) ? "tab tab-active" : "tab";
}
