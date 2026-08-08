using System.Text;

using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity;

public record ConfirmEmail(string UserId, string EncodedCode);

public static class ConfirmEmailHandler
{
    public static async Task<IdentityResultDto> Handle(ConfirmEmail command, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EncodedCode));
        var result = await userManager.ConfirmEmailAsync(user, code);
        return result.ToDto();
    }
}
