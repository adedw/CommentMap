using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Commands;

public record SetPasswordCommand(Guid UserId, string NewPassword) : ICommand<IdentityResultDto>;

public sealed class SetPasswordHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : ICommandHandler<SetPasswordCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.UserNotFound();

        var result = await userManager.AddPasswordAsync(user, command.NewPassword);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}
