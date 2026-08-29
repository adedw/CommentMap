using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record CreateExternalUser(string UserName, ExternalLoginInfo LoginInfo);

public static class CreateExternalUserHandler
{
    public static async Task<IdentityResultDto> Handle(
        CreateExternalUser command,
        UserManager<User> userManager,
        IUserStore<User> userStore,
        SignInManager<User> signInManager,
        ILogger<CreateExternalUser> logger,
        CancellationToken cancellationToken)
    {
        var user = new User();
        await userStore.SetUserNameAsync(user, command.UserName, cancellationToken);

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            return result.ToDto();

        result = await userManager.AddLoginAsync(user, command.LoginInfo);
        if (!result.Succeeded)
            return result.ToDto();

        logger.LogInformation("User created an account using {Name} provider.", command.LoginInfo.LoginProvider);
        await signInManager.SignInAsync(user, isPersistent: false, command.LoginInfo.LoginProvider);
        return IdentityResultDto.Success();
    }
}
