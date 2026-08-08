using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CommentMap.Application.Features.Identity;

public record HasPassword(Guid UserId);
public record HasPasswordResult(bool Found, bool HasPassword);

public static class HasPasswordHandler
{
    public static async Task<HasPasswordResult> Handle(HasPassword query, UserManager<User> userManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new HasPasswordResult(false, false);

        return new HasPasswordResult(true, await userManager.HasPasswordAsync(user));
    }
}

public record ChangePassword(Guid UserId, string OldPassword, string NewPassword);

public static class ChangePasswordHandler
{
    public static async Task<IdentityResultDto> Handle(
        ChangePassword command,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<ChangePassword> logger)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var result = await userManager.ChangePasswordAsync(user, command.OldPassword, command.NewPassword);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        logger.LogInformation("User changed their password successfully.");
        return IdentityResultDto.Success();
    }
}

public record SetPassword(Guid UserId, string NewPassword);

public static class SetPasswordHandler
{
    public static async Task<IdentityResultDto> Handle(
        SetPassword command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var result = await userManager.AddPasswordAsync(user, command.NewPassword);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}
