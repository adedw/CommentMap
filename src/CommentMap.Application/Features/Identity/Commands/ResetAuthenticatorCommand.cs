using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record ResetAuthenticatorCommand(Guid UserId) : ICommand<bool>;

public sealed class ResetAuthenticatorHandler(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<ResetAuthenticatorCommand> logger)
    : ICommandHandler<ResetAuthenticatorCommand, bool>
{
    public async Task<bool> Handle(ResetAuthenticatorCommand command, CancellationToken cancellationToken)
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
