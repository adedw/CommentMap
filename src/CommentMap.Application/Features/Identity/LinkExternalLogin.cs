using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record LinkExternalLogin(Guid UserId);

public static class LinkExternalLoginHandler
{
    public static async Task<IdentityResultDto> Handle(
        LinkExternalLogin command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var userId = await userManager.GetUserIdAsync(user);
        var info = await signInManager.GetExternalLoginInfoAsync(userId);
        if (info is null)
            throw new InvalidOperationException("Unexpected error occurred loading external login info.");

        var result = await userManager.AddLoginAsync(user, info);
        return result.ToDto();
    }
}
