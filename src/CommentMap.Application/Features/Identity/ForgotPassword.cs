using CommentMap.Application.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CommentMap.Application.Features.Identity;

public record ForgotPassword(string Email);

public record ForgotPasswordResult(bool UserFound, Guid? UserId, string? EncodedResetCode);

public static class ForgotPasswordHandler
{
    public static async Task<ForgotPasswordResult> Handle(ForgotPassword command, UserManager<User> userManager)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return new ForgotPasswordResult(false, null, null);

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return new ForgotPasswordResult(true, user.Id, encoded);
    }
}
