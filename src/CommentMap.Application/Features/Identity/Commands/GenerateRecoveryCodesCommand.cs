using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record GenerateRecoveryCodesCommand(Guid UserId) : ICommand<GenerateRecoveryCodesResult>;
public record GenerateRecoveryCodesResult(bool Found, bool TwoFactorEnabled, string[]? RecoveryCodes);

public sealed class GenerateRecoveryCodesHandler(UserManager<User> userManager, ILogger<GenerateRecoveryCodesCommand> logger)
    : ICommandHandler<GenerateRecoveryCodesCommand, GenerateRecoveryCodesResult>
{
    public async Task<GenerateRecoveryCodesResult> Handle(GenerateRecoveryCodesCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return new GenerateRecoveryCodesResult(false, false, null);

        if (!await userManager.GetTwoFactorEnabledAsync(user))
            throw new InvalidOperationException("Cannot generate recovery codes for user because they do not have 2FA enabled.");

        var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", user.Id);
        return new GenerateRecoveryCodesResult(true, true, recoveryCodes!.ToArray());
    }
}
