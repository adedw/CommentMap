using System.Security.Claims;

namespace CommentMap.Mvc.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid FindUserId(this ClaimsPrincipal user)
    {
        if (Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
        {
            return userId;
        }
        throw new InvalidOperationException("User id claim not found or not in Guid format.");
    }
}