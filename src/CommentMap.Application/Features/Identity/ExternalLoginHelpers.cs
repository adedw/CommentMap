using System.Security.Claims;

namespace CommentMap.Application.Features.Identity;

public static class ExternalLoginHelpers
{
    public static string? SuggestedUserName(ClaimsPrincipal principal) =>
        principal.HasClaim(c => c.Type == ClaimTypes.Name)
            ? principal.FindFirstValue(ClaimTypes.Name)
            : null;
}
