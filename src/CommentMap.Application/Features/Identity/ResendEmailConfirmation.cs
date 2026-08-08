using System.Text;

using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity;

public record ResendEmailConfirmation(string Email);

public record ResendEmailConfirmationResult(bool UserFound, Guid? UserId, string? EncodedCode);

public static class ResendEmailConfirmationHandler
{
    public static async Task<ResendEmailConfirmationResult> Handle(
        ResendEmailConfirmation command,
        UserManager<User> userManager)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return new ResendEmailConfirmationResult(false, null, null);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return new ResendEmailConfirmationResult(true, user.Id, encoded);
    }
}
