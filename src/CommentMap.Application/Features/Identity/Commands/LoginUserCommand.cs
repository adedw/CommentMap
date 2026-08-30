using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record LoginUserCommand(string Email, string Password, bool RememberMe) : ICommand<LoginResultDto>;

public sealed class LoginUserHandler(SignInManager<User> signInManager, ILogger<LoginUserCommand> logger)
    : ICommandHandler<LoginUserCommand, LoginResultDto>
{
    public async Task<LoginResultDto> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var result = await signInManager.PasswordSignInAsync(
            command.Email,
            command.Password,
            command.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
            logger.LogInformation("User logged in.");
        else if (result.IsLockedOut)
            logger.LogWarning("User account locked out.");

        return result.ToDto();
    }
}
