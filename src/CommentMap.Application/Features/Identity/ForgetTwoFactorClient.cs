using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record ForgetTwoFactorClient(Guid UserId);

public static class ForgetTwoFactorClientHandler
{
    public static async Task<bool> Handle(
        ForgetTwoFactorClient command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return false;

        await signInManager.ForgetTwoFactorClientAsync();
        return true;
    }
}
