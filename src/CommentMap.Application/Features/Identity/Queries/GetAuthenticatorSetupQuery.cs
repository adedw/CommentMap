using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Queries;

public record GetAuthenticatorSetupQuery(Guid UserId) : IQuery<AuthenticatorSetupDto?>;

public sealed class GetAuthenticatorSetupHandler(UserManager<User> userManager, IAuthenticatorSetupProvider authenticatorSetup)
    : IQueryHandler<GetAuthenticatorSetupQuery, AuthenticatorSetupDto?>
{
    public async Task<AuthenticatorSetupDto?> Handle(GetAuthenticatorSetupQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return null;

        return await authenticatorSetup.CreateAsync(user, cancellationToken);
    }
}
