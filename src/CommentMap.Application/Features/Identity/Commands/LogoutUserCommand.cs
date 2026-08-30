using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record LogoutUserCommand : ICommand;

public sealed class LogoutUserHandler(SignInManager<User> signInManager, ILogger<LogoutUserCommand> logger)
    : ICommandHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand message, CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("User logged out.");
    }
}
