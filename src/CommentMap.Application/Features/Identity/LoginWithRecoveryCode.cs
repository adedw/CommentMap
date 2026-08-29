using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record LoginWithRecoveryCode(string RecoveryCode);

public static class LoginWithRecoveryCodeHandler
{
    public static async Task<LoginResultDto> Handle(
        LoginWithRecoveryCode command,
        SignInManager<User> signInManager,
        ILogger<LoginWithRecoveryCode> logger)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync()
            ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        var recoveryCode = command.RecoveryCode.Replace(" ", string.Empty);
        var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded)
            logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
        else if (result.IsLockedOut)
            logger.LogWarning("User account locked out.");

        return result.ToDto();
    }
}
