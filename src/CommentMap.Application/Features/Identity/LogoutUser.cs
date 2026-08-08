using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record LogoutUser;

public static class LogoutUserHandler
{
    public static async Task Handle(LogoutUser _, SignInManager<User> signInManager, ILogger<LogoutUser> logger)
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("User logged out.");
    }
}
