using System.Text.Encodings.Web;

using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using QRCoder;

namespace CommentMap.Application.Features.Identity;

public record EnableAuthenticator(Guid UserId, string Code, string AppName = "CommentMap");

public static class EnableAuthenticatorHandler
{
    public static async Task<EnableAuthenticatorResultDto> Handle(
        EnableAuthenticator command,
        UserManager<User> userManager,
        ILogger<EnableAuthenticator> logger,
        UrlEncoder urlEncoder,
        QRCodeGenerator qrCodeGenerator)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return new EnableAuthenticatorResultDto(false, false, false, null, null);

        var verificationCode = command.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!isValid)
        {
            var setup = await GetAuthenticatorSetupHandler.Handle(
                new GetAuthenticatorSetup(command.UserId, command.AppName),
                userManager, urlEncoder, qrCodeGenerator);
            return new EnableAuthenticatorResultDto(false, true, false, null, setup);
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);

        if (await userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return new EnableAuthenticatorResultDto(true, false, true, recoveryCodes!.ToArray(), null);
        }

        return new EnableAuthenticatorResultDto(true, false, false, null, null);
    }
}
