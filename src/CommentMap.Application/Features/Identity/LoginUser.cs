using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record LoginUser(string Email, string Password, bool RememberMe);

public static class LoginUserHandler
{
    public static async Task<LoginResultDto> Handle(
        LoginUser command,
        SignInManager<User> signInManager,
        ILogger<LoginUser> logger)
    {
        var result = await signInManager.PasswordSignInAsync(
            command.Email,
            command.Password,
            command.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
            logger.LogInformation("User logged in.");
        else if (result.IsLockedOut)
            logger.LogWarning("User account locked out.");

        return result.ToDto();
    }
}
