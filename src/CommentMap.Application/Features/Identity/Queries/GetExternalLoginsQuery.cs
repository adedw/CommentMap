using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Queries;

public record GetExternalLoginsQuery(Guid UserId) : IQuery<ExternalLoginsDto>;
public record ExternalLoginsDto(
    bool Found,
    IReadOnlyList<UserLoginInfo> CurrentLogins,
    IReadOnlyList<string> OtherLoginProviderNames,
    bool ShowRemoveButton);

public sealed class GetExternalLoginsHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IUserStore<User> userStore) : IQueryHandler<GetExternalLoginsQuery, ExternalLoginsDto>
{
    public async Task<ExternalLoginsDto> Handle(GetExternalLoginsQuery query, CancellationToken cancellationToken)
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
