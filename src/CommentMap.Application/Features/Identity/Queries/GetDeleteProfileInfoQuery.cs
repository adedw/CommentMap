using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Queries;

public record GetDeleteProfileInfoQuery(Guid UserId) : IQuery<GetDeleteProfileInfoResult>;
public record GetDeleteProfileInfoResult(bool Found, bool RequirePassword);

public sealed class GetDeleteProfileInfoHandler(UserManager<User> userManager)
    : IQueryHandler<GetDeleteProfileInfoQuery, GetDeleteProfileInfoResult>
{
    public async Task<GetDeleteProfileInfoResult> Handle(GetDeleteProfileInfoQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new GetDeleteProfileInfoResult(false, false);

        return new GetDeleteProfileInfoResult(true, await userManager.HasPasswordAsync(user));
    }
}
