using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Queries;

public record HasPasswordQuery(Guid UserId) : IQuery<HasPasswordResult>;
public record HasPasswordResult(bool Found, bool HasPassword);

public sealed class HasPasswordHandler(UserManager<User> userManager) : IQueryHandler<HasPasswordQuery, HasPasswordResult>
{
    public async Task<HasPasswordResult> Handle(HasPasswordQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new HasPasswordResult(false, false);

        return new HasPasswordResult(true, await userManager.HasPasswordAsync(user));
    }
}
