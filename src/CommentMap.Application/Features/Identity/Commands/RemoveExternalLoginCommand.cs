using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Commands;

public record RemoveExternalLoginCommand(Guid UserId, string LoginProvider, string ProviderKey) : ICommand<IdentityResultDto>;

public sealed class RemoveExternalLoginHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : ICommandHandler<RemoveExternalLoginCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(RemoveExternalLoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.UserNotFound();

        var result = await userManager.RemoveLoginAsync(user, command.LoginProvider, command.ProviderKey);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}
