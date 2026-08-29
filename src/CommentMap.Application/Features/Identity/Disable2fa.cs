using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record Disable2fa(Guid UserId);

public static class Disable2faHandler
{
    public static async Task<bool> Handle(Disable2fa command, UserManager<User> userManager, ILogger<Disable2fa> logger)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return false;

        if (!await userManager.GetTwoFactorEnabledAsync(user))
            throw new InvalidOperationException("Cannot disable 2FA for user as it's not currently enabled.");

        var result = await userManager.SetTwoFactorEnabledAsync(user, false);
        if (!result.Succeeded)
            throw new InvalidOperationException("Unexpected error occurred disabling 2FA.");

        logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", user.Id);
        return true;
    }
}
