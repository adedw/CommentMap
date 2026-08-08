using System.Text;

using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity;

public record ConfirmEmailChange(string UserId, string Email, string EncodedCode);

public static class ConfirmEmailChangeHandler
{
    public static async Task<IdentityResultDto> Handle(
        ConfirmEmailChange command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EncodedCode));
        var result = await userManager.ChangeEmailAsync(user, command.Email, code);
        if (!result.Succeeded)
            return result.ToDto();

        var setUserNameResult = await userManager.SetUserNameAsync(user, command.Email);
        if (!setUserNameResult.Succeeded)
            return setUserNameResult.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}
