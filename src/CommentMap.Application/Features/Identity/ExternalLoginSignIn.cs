using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record ExternalLoginSignIn(string LoginProvider, string ProviderKey);

public static class ExternalLoginSignInHandler
{
    public static async Task<LoginResultDto> Handle(
        ExternalLoginSignIn command,
        SignInManager<User> signInManager,
        ILogger<ExternalLoginSignIn> logger)
    {
        var result = await signInManager.ExternalLoginSignInAsync(
            command.LoginProvider,
            command.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
            logger.LogInformation("User logged in with {LoginProvider} provider.", command.LoginProvider);

        return result.ToDto();
    }
}
