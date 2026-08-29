using CommentMap.Application.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Comments.Queries;

public record GetCommentTitleQuery(Guid Id) : IQuery<string?>;

public sealed class GetCommentTitleHandler(ICommentMapDbContext db) : IQueryHandler<GetCommentTitleQuery, string?>
{
    public Task<string?> Handle(GetCommentTitleQuery query, CancellationToken cancellationToken)
    {
        return db.Comments
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(c => c.Title)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
