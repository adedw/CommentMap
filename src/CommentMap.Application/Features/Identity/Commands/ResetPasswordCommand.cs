using System.Text;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity.Commands;

public record ResetPasswordCommand(string UserId, string EncodedCode, string Password) : ICommand<IdentityResultDto>;

public sealed class ResetPasswordHandler(UserManager<User> userManager) : ICommandHandler<ResetPasswordCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
            return IdentityResultDto.Success();

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EncodedCode));
        var result = await userManager.ResetPasswordAsync(user, code, command.Password);
        return result.ToDto();
    }
}
