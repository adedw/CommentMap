using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Commands;

public record SignInAfterRegistrationCommand(Guid UserId) : ICommand;

public sealed class SignInAfterRegistrationHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : ICommandHandler<SignInAfterRegistrationCommand>
{
    public async Task Handle(SignInAfterRegistrationCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString())
            ?? throw new InvalidOperationException("User not found.");
        await signInManager.SignInAsync(user, isPersistent: false);
    }
}
