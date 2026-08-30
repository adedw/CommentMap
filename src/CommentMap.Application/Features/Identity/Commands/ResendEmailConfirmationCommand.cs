using System.Text;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity.Commands;

public record ResendEmailConfirmationCommand(string Email) : ICommand<ResendEmailConfirmationResult>;

public record ResendEmailConfirmationResult(bool UserFound, Guid? UserId, string? EncodedCode);

public sealed class ResendEmailConfirmationHandler(UserManager<User> userManager)
    : ICommandHandler<ResendEmailConfirmationCommand, ResendEmailConfirmationResult>
{
    public async Task<ResendEmailConfirmationResult> Handle(ResendEmailConfirmationCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
            return new ResendEmailConfirmationResult(false, null, null);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return new ResendEmailConfirmationResult(true, user.Id, encoded);
    }
}
