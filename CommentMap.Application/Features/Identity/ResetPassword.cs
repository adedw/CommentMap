using CommentMap.Application.Entities;
using CommentMap.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CommentMap.Application.Features.Identity;

public record ResetPassword(string UserId, string EncodedCode, string Password);

public static class ResetPasswordHandler
{
    public static async Task<IdentityResultDto> Handle(ResetPassword command, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
            return IdentityResultDto.Success();

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EncodedCode));
        var result = await userManager.ResetPasswordAsync(user, code, command.Password);
        return result.ToDto();
    }
}
