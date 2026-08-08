using CommentMap.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CommentMap.Mvc.ViewComponents;

/// <summary>
/// Renders a status message alert read from TempData, or nothing when no message is set.
/// The message is consumed as it is read, so it appears only once (e.g. after a redirect).
/// </summary>
public class StatusAlertViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var message = TempData.GetStatus();
        if (message is null)
            return Content(string.Empty);

        return View(message);
    }
}