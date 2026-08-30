using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record ExternalLoginSignInCommand(string LoginProvider, string ProviderKey) : ICommand<LoginResultDto>;

public sealed class ExternalLoginSignInHandler(SignInManager<User> signInManager, ILogger<ExternalLoginSignInCommand> logger)
    : ICommandHandler<ExternalLoginSignInCommand, LoginResultDto>
{
    public async Task<LoginResultDto> Handle(ExternalLoginSignInCommand command, CancellationToken cancellationToken)
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
