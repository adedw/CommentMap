using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Commands;

public record ForgetTwoFactorClientCommand(Guid UserId) : ICommand<bool>;

public sealed class ForgetTwoFactorClientHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : ICommandHandler<ForgetTwoFactorClientCommand, bool>
{
    public async Task<bool> Handle(ForgetTwoFactorClientCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return false;

        await signInManager.ForgetTwoFactorClientAsync();
        return true;
    }
}
