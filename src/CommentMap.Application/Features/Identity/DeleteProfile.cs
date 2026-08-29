using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record DeleteProfile(Guid UserId, string? Password);

public static class DeleteProfileHandler
{
    public static async Task<IdentityResultDto> Handle(
        DeleteProfile command,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<DeleteProfile> logger)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        if (await userManager.HasPasswordAsync(user))
        {
            if (command.Password is null || !await userManager.CheckPasswordAsync(user, command.Password))
                return IdentityResultDto.Failed([new IdentityErrorDto("Password", "Incorrect password.")]);
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Unexpected error occurred deleting user.");

        await signInManager.SignOutAsync();
        logger.LogInformation("User with ID '{UserId}' deleted themselves.", user.Id);
        return IdentityResultDto.Success();
    }
}
