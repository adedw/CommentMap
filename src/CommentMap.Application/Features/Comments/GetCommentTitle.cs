using CommentMap.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Comments;

public record GetCommentTitle(Guid Id);

public static class GetCommentTitleHandler
{
    public static Task<string?> Handle(
        GetCommentTitle query,
        ICommentMapDbContext db,
        CancellationToken cancellationToken)
    {
        return db.Comments
            .AsNoTracking()
            .Where(c => c.Id == query.Id)
            .Select(c => c.Title)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
