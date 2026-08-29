using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Features.Identity.Queries;

public record GetProfileEmailQuery(Guid UserId) : IQuery<GetProfileEmailResult>;
public record GetProfileEmailResult(bool Found, string? Email);

public sealed class GetProfileEmailHandler(UserManager<User> userManager) : IQueryHandler<GetProfileEmailQuery, GetProfileEmailResult>
{
    public async Task<GetProfileEmailResult> Handle(GetProfileEmailQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());
        if (user is null)
            return new GetProfileEmailResult(false, null);

        return new GetProfileEmailResult(true, await userManager.GetEmailAsync(user));
    }
}
