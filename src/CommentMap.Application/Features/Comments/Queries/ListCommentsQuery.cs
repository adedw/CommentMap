using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;

using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Comments.Queries;

public record ListCommentsQuery(Guid UserId, CommentSort Sort) : IQuery<List<CommentCardDto>>;

public sealed class ListCommentsHandler(ICommentMapDbContext db) : IQueryHandler<ListCommentsQuery, List<CommentCardDto>>
{
    public async Task<List<CommentCardDto>> Handle(ListCommentsQuery query, CancellationToken cancellationToken)
    {
        var commentsQuery = db.Comments
            .AsNoTracking()
            .Where(c => c.UserId == query.UserId)
            .Where(c => !c.IsDeleted);

        commentsQuery = OrderBy(commentsQuery, query.Sort);

        return await commentsQuery
            .Select(c => new CommentCardDto(
                c.Id,
                c.Location.X,
                c.Location.Y,
                c.Title,
                c.Text,
                c.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Comment> OrderBy(IQueryable<Comment> queryable, CommentSort order) =>
        order switch
        {
            CommentSort.CreatedAt => queryable.OrderByDescending(c => c.CreatedAt),
            CommentSort.Title => queryable.OrderBy(c => c.Title),
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, "Unexpected order value."),
        };
}
