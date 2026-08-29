using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record GetDeleteProfileInfo(Guid UserId);
public record GetDeleteProfileInfoResult(bool Found, bool RequirePassword);

public static class GetDeleteProfileInfoHandler
{
    public static async Task<GetDeleteProfileInfoResult> Handle(
        GetDeleteProfileInfo query,
        UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new GetDeleteProfileInfoResult(false, false);

        return new GetDeleteProfileInfoResult(true, await userManager.HasPasswordAsync(user));
    }
}
