using CommentMap.Application.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CommentMap.Application.Features.Identity;

public record GetProfileEmail(Guid UserId);
public record GetProfileEmailResult(bool Found, string? Email);

public static class GetProfileEmailHandler
{
    public static async Task<GetProfileEmailResult> Handle(GetProfileEmail query, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new GetProfileEmailResult(false, null);

        return new GetProfileEmailResult(true, await userManager.GetEmailAsync(user));
    }
}

public record RequestEmailChange(Guid UserId, string NewEmail);
public record RequestEmailChangeResult(bool Found, bool Unchanged, string? EncodedCode);

public static class RequestEmailChangeHandler
{
    public static async Task<RequestEmailChangeResult> Handle(
        RequestEmailChange command,
        UserManager<User> userManager)
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
