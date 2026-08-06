using CommentMap.Application.Entities;
using CommentMap.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CommentMap.Application.Features.Identity;

public record ExternalLoginSignIn(string LoginProvider, string ProviderKey);

public static class ExternalLoginSignInHandler
{
    public static async Task<LoginResultDto> Handle(
        ExternalLoginSignIn command,
        SignInManager<User> signInManager,
        ILogger<ExternalLoginSignIn> logger)
    {
        var result = await signInManager.ExternalLoginSignInAsync(
            command.LoginProvider,
            command.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
            logger.LogInformation("User logged in with {LoginProvider} provider.", command.LoginProvider);

        return result.ToDto();
    }
}

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

public record GetExternalLogins(Guid UserId);
public record ExternalLoginsDto(
    bool Found,
    IReadOnlyList<UserLoginInfo> CurrentLogins,
    IReadOnlyList<string> OtherLoginProviderNames,
    bool ShowRemoveButton);

public static class GetExternalLoginsHandler
{
    public static async Task<ExternalLoginsDto> Handle(
        GetExternalLogins query,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserStore<User> userStore,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new ExternalLoginsDto(false, [], [], false);

        var currentLogins = await userManager.GetLoginsAsync(user);
        var otherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
            .Select(a => a.Name!)
            .ToList();

        string? passwordHash = null;
        if (userStore is IUserPasswordStore<User> userPasswordStore)
            passwordHash = await userPasswordStore.GetPasswordHashAsync(user, cancellationToken);

        var showRemove = passwordHash != null || currentLogins.Count > 1;
        return new ExternalLoginsDto(true, currentLogins.ToList(), otherLogins, showRemove);
    }
}

public record RemoveExternalLogin(Guid UserId, string LoginProvider, string ProviderKey);

public static class RemoveExternalLoginHandler
{
    public static async Task<IdentityResultDto> Handle(
        RemoveExternalLogin command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var result = await userManager.RemoveLoginAsync(user, command.LoginProvider, command.ProviderKey);
        if (!result.Succeeded)
            return result.ToDto();

        await signInManager.RefreshSignInAsync(user);
        return IdentityResultDto.Success();
    }
}

public record LinkExternalLogin(Guid UserId);

public static class LinkExternalLoginHandler
{
    public static async Task<IdentityResultDto> Handle(
        LinkExternalLogin command,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return IdentityResultDto.Failed([new IdentityErrorDto("UserNotFound", "Unable to load user.")]);

        var userId = await userManager.GetUserIdAsync(user);
        var info = await signInManager.GetExternalLoginInfoAsync(userId);
        if (info is null)
            throw new InvalidOperationException("Unexpected error occurred loading external login info.");

        var result = await userManager.AddLoginAsync(user, info);
        return result.ToDto();
    }
}

public static class ExternalLoginHelpers
{
    public static string? SuggestedUserName(ClaimsPrincipal principal) =>
        principal.HasClaim(c => c.Type == ClaimTypes.Name)
            ? principal.FindFirstValue(ClaimTypes.Name)
            : null;
}
