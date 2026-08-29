using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity;

public record GetTwoFactorStatus(Guid UserId);
public record TwoFactorStatusDto(
    bool Found,
    bool HasAuthenticator,
    bool Is2faEnabled,
    bool IsMachineRemembered,
    int RecoveryCodesLeft);

public static class GetTwoFactorStatusHandler
{
    public static async Task<TwoFactorStatusDto> Handle(
        GetTwoFactorStatus query,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new TwoFactorStatusDto(false, false, false, false, 0);

        return new TwoFactorStatusDto(
            true,
            await userManager.GetAuthenticatorKeyAsync(user) != null,
            await userManager.GetTwoFactorEnabledAsync(user),
            await signInManager.IsTwoFactorClientRememberedAsync(user),
            await userManager.CountRecoveryCodesAsync(user));
    }
}
