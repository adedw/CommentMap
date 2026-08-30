using System.Text;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity.Commands;

public record ConfirmEmailCommand(string UserId, string EncodedCode) : ICommand<IdentityResultDto>;

public sealed class ConfirmEmailHandler(UserManager<User> userManager) : ICommandHandler<ConfirmEmailCommand, IdentityResultDto>
{
    public async Task<IdentityResultDto> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId);
        if (user is null)
            return IdentityResultDto.UserNotFound();

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(command.EncodedCode));
        var result = await userManager.ConfirmEmailAsync(user, code);
        return result.ToDto();
    }
}
