using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record LoginWith2FACommand(string TwoFactorCode, bool RememberMe, bool RememberMachine) : ICommand<LoginResultDto>;

public sealed class LoginWith2FAHandler(SignInManager<User> signInManager, ILogger<LoginWith2FACommand> logger)
    : ICommandHandler<LoginWith2FACommand, LoginResultDto>
{
    public async Task<LoginResultDto> Handle(LoginWith2FACommand command, CancellationToken cancellationToken)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync()
            ?? throw new InvalidOperationException("Unable to load two-factor authentication user.");

        var authenticatorCode = command.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(
            authenticatorCode, command.RememberMe, command.RememberMachine);

        if (result.Succeeded)
            logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
        else if (result.IsLockedOut)
            logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);

        return result.ToDto();
    }
}
