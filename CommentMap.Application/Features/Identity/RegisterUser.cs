using CommentMap.Application.Entities;
using CommentMap.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace CommentMap.Application.Features.Identity;

public record RegisterUser(string Email, string Password);

public record RegisterUserResult(
    bool Succeeded,
    Guid? UserId,
    string? EncodedEmailConfirmationCode,
    bool RequireConfirmedAccount,
    IReadOnlyList<IdentityErrorDto> Errors);

public static class RegisterUserHandler
{
    public static async Task<RegisterUserResult> Handle(
        RegisterUser command,
        UserManager<User> userManager,
        IUserStore<User> userStore,
        CancellationToken cancellationToken)
    {
        var user = new User();
        await userStore.SetUserNameAsync(user, command.Email, cancellationToken);
        var emailStore = (IUserEmailStore<User>)userStore;
        await emailStore.SetEmailAsync(user, command.Email, cancellationToken);

        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            return new RegisterUserResult(false, null, null, false, result.ToDto().Errors);
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        return new RegisterUserResult(
            true,
            user.Id,
            encoded,
            userManager.Options.SignIn.RequireConfirmedAccount,
            []);
    }
}
