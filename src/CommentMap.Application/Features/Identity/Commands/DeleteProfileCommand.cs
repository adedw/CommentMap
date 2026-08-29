using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record DeleteProfileCommand(Guid UserId, string? Password) : ICommand<IdentityResultDto>;

public sealed class DeleteProfileHandler(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<DeleteProfileCommand> logger)
    : ICommandHandler<DeleteProfileCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(DeleteProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.UserNotFound();

        if (await userManager.HasPasswordAsync(user))
        {
            if (command.Password is null || !await userManager.CheckPasswordAsync(user, command.Password))
                return IdentityResultDto.IncorrectPassword();
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Unexpected error occurred deleting user.");

        await signInManager.SignOutAsync();
        logger.LogInformation("User with ID '{UserId}' deleted themselves.", user.Id);
        return IdentityResultDto.Success();
    }
}
