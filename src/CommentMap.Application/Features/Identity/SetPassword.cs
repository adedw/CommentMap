using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record SetPassword(Guid UserId, string NewPassword);

public static class SetPasswordHandler
{
    public static async Task<IdentityResultDto> Handle(
        SetPassword command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var result = await userManager.AddPasswordAsync(user, command.NewPassword);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}
