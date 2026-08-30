using System.Text;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity.Queries;

public record ForgotPasswordQuery(string Email) : IQuery<ForgotPasswordResult>;

public record ForgotPasswordResult(bool UserFound, Guid? UserId, string? EncodedResetCode);

public sealed class ForgotPasswordHandler(UserManager<User> userManager) : IQueryHandler<ForgotPasswordQuery, ForgotPasswordResult>
{
    public async Task<ForgotPasswordResult> Handle(ForgotPasswordQuery command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
            return new ForgotPasswordResult(false, null, null);

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        return new ForgotPasswordResult(true, user.Id, encoded);
    }
}
