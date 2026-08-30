using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Commands;

public record LinkExternalLoginCommand(Guid UserId) : ICommand<IdentityResultDto>;

public sealed class LinkExternalLoginHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : ICommandHandler<LinkExternalLoginCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(LinkExternalLoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.UserNotFound();

        var userId = await userManager.GetUserIdAsync(user);
        var info = await signInManager.GetExternalLoginInfoAsync(userId);
        if (info is null)
            throw new InvalidOperationException("Unexpected error occurred loading external login info.");

        var result = await userManager.AddLoginAsync(user, info);
        return result.ToDto();
    }
}
