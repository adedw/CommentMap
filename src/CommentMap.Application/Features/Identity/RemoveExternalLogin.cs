using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record RemoveExternalLogin(Guid UserId, string LoginProvider, string ProviderKey);

public static class RemoveExternalLoginHandler
{
    public static async Task<IdentityResultDto> Handle(
        RemoveExternalLogin command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var result = await userManager.RemoveLoginAsync(user, command.LoginProvider, command.ProviderKey);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}
