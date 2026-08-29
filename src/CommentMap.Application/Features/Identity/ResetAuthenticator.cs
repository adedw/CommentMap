using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record ResetAuthenticator(Guid UserId);

public static class ResetAuthenticatorHandler
{
    public static async Task<bool> Handle(
        ResetAuthenticator command,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<ResetAuthenticator> logger)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return false;

        await userManager.SetTwoFactorEnabledAsync(user, false);
        await userManager.ResetAuthenticatorKeyAsync(user);
        logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);
        await signInManager.RefreshSignInAsync(user);
        return true;
    }
}
