using CommentMap.Application.Features.Comments;
using CommentMap.Application.Models;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Wolverine;

namespace CommentMap.Mvc.Pages.Comments;

[Authorize]
public class IndexModel(IMessageBus bus) : PageModel
{
    private const string SortCookieName = "CM.Comments.Sort";

    private static readonly CookieOptions SortCookieOptions = new()
    {
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddYears(1),
    };

    public List<CommentCardViewModel>? Comments { get; private set; }

    [BindProperty(SupportsGet = true)]
    public CommentSort? Sort { get; set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindUserId();
        Sort = ResolveSort();

        var items = await bus.InvokeAsync<List<CommentCardDto>>(
            new ListComments(userId, Sort.Value), cancellationToken);

        Comments = [.. items.Select(c => new CommentCardViewModel(
            c.Id,
            c.Longitude,
            c.Latitude,
            c.Title,
            c.Text,
            c.CreatedAt))];

        return Page();
    }

    /// <summary>
    /// Determines the effective comment sort order using, in priority order:
    /// the <c>Sort</c> query parameter, the persisted "CM.Comments.Sort" cookie,
    /// then <see cref="CommentSort.CreatedAt"/> as the default.
    /// </summary>
    /// <remarks>
    /// When the query parameter is present and differs from the stored cookie,
    /// the cookie is refreshed to persist the user's choice across requests.
    /// </remarks>
    /// <returns>The resolved <see cref="CommentSort"/> to apply.</returns>
    private CommentSort ResolveSort()
    {
        var cookieSort = Request.Cookies[SortCookieName];

        if (Sort is { } querySort)
        {
            if (!string.Equals(cookieSort, querySort.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                Response.Cookies.Append(SortCookieName, querySort.ToString(), SortCookieOptions);
            }

            return querySort;
        }

        return Enum.TryParse<CommentSort>(cookieSort, out var sort)
            ? sort
            : CommentSort.CreatedAt;
    }
}
