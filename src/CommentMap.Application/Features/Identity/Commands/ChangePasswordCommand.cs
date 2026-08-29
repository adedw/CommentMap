using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity.Commands;

public record ChangePasswordCommand(Guid UserId, string OldPassword, string NewPassword) : ICommand<IdentityResultDto>;

public sealed class ChangePasswordHandler(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<ChangePasswordCommand> logger)
    : ICommandHandler<ChangePasswordCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.UserNotFound();

        var result = await userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        logger.LogInformation("User changed their password successfully.");
        return IdentityResultDto.Success();
    }
}
