using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record EnableAuthenticatorCommand(Guid UserId, string Code) : ICommand<EnableAuthenticatorResultDTO>;

public sealed class EnableAuthenticatorHandler(
    UserManager<User> userManager,
    ILogger<EnableAuthenticatorCommand> logger,
    IAuthenticatorSetupProvider authenticatorSetup) : ICommandHandler<EnableAuthenticatorCommand, EnableAuthenticatorResultDTO>
{
    public async Task<EnableAuthenticatorResultDTO> Handle(EnableAuthenticatorCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return new EnableAuthenticatorResultDTO(false, false, false, null, null);

        var verificationCode = command.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!isValid)
        {
            var setup = await authenticatorSetup.CreateAsync(user, cancellationToken);
            return new EnableAuthenticatorResultDTO(false, true, false, null, setup);
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", user.Id);

        if (await userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return new EnableAuthenticatorResultDTO(true, false, true, recoveryCodes!.ToArray(), null);
        }

        return new EnableAuthenticatorResultDTO(true, false, false, null, null);
    }
}
