using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Queries;

public record GetTwoFactorStatusQuery(Guid UserId) : IQuery<TwoFactorStatusDto>;
public record TwoFactorStatusDto(
    bool Found,
    bool HasAuthenticator,
    bool Is2faEnabled,
    bool IsMachineRemembered,
    int RecoveryCodesLeft);

public sealed class GetTwoFactorStatusHandler(UserManager<User> userManager, SignInManager<User> signInManager)
    : IQueryHandler<GetTwoFactorStatusQuery, TwoFactorStatusDto>
{
    public async Task<TwoFactorStatusDto> Handle(GetTwoFactorStatusQuery query, CancellationToken cancellationToken)
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
