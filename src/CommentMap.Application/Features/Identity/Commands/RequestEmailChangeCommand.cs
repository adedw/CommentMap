using System.Text;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity.Commands;

public record RequestEmailChangeCommand(Guid UserId, string NewEmail) : ICommand<RequestEmailChangeResult>;
public record RequestEmailChangeResult(bool Found, bool Unchanged, string? EncodedCode);

public sealed class RequestEmailChangeHandler(UserManager<User> userManager)
    : ICommandHandler<RequestEmailChangeCommand, RequestEmailChangeResult>
{
    public async Task<RequestEmailChangeResult> Handle(RequestEmailChangeCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return new RequestEmailChangeResult(false, false, null);

        var email = await userManager.GetEmailAsync(user);
        if (command.NewEmail == email)
            return new RequestEmailChangeResult(true, true, null);

        var code = await userManager.GenerateChangeEmailTokenAsync(user, command.NewEmail);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return new RequestEmailChangeResult(true, false, encoded);
    }
}
