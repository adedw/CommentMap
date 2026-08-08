using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record GetDeleteProfileInfo(Guid UserId);
public record GetDeleteProfileInfoResult(bool Found, bool RequirePassword);

public static class GetDeleteProfileInfoHandler
{
    public static async Task<GetDeleteProfileInfoResult> Handle(
        GetDeleteProfileInfo query,
        UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new GetDeleteProfileInfoResult(false, false);

        return new GetDeleteProfileInfoResult(true, await userManager.HasPasswordAsync(user));
    }
}

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
