using System.Text;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CommentMap.Application.Features.Identity.Commands;

public record RegisterUserCommand(string Email, string Password) : ICommand<RegisterUserResult>;

public record RegisterUserResult(
    bool Succeeded,
    Guid? UserId,
    string? EncodedEmailConfirmationCode,
    bool RequireConfirmedAccount,
    IReadOnlyList<IdentityErrorDto> Errors);

public sealed class RegisterUserHandler(UserManager<User> userManager, IUserStore<User> userStore)
    : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
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
