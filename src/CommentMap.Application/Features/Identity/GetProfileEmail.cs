using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record GetProfileEmail(Guid UserId);
public record GetProfileEmailResult(bool Found, string? Email);

public static class GetProfileEmailHandler
{
    public static async Task<GetProfileEmailResult> Handle(GetProfileEmail query, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new GetProfileEmailResult(false, null);

        return new GetProfileEmailResult(true, await userManager.GetEmailAsync(user));
    }
}
