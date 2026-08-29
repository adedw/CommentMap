using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record HasPassword(Guid UserId);
public record HasPasswordResult(bool Found, bool HasPassword);

public static class HasPasswordHandler
{
    public static async Task<HasPasswordResult> Handle(HasPassword query, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new HasPasswordResult(false, false);

        return new HasPasswordResult(true, await userManager.HasPasswordAsync(user));
    }
}
