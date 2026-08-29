using System.Text;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity.Commands;

public record ConfirmEmailChangeCommand(string UserId, string Email, string EncodedCode) : ICommand<IdentityResultDto>;

public sealed class ConfirmEmailChangeHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : ICommandHandler<ConfirmEmailChangeCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(ConfirmEmailChangeCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
            return IdentityResultDto.UserNotFound();

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EncodedCode));
        var result = await userManager.ChangeEmailAsync(user, command.Email, code);
        if (!result.Succeeded)
            return result.ToDto();

        var setUserNameResult = await userManager.SetUserNameAsync(user, command.Email);
        if (!setUserNameResult.Succeeded)
            return setUserNameResult.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}
