using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record SignInAfterRegistration(Guid UserId);

public static class SignInAfterRegistrationHandler
{
    public static async Task Handle(SignInAfterRegistration command, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString())
            ?? throw new InvalidOperationException("User not found.");
        await signInManager.SignInAsync(user, isPersistent: false);
    }
}
