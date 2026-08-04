using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Comments;

public record ListComments(Guid UserId, Order Order);

public static class ListCommentsHandler
{
    public static async Task<List<CommentCardDto>> Handle(
        ListComments query,
        ICommentMapDbContext db,
        CancellationToken cancellationToken)
    {
        var commentsQuery = db.Comments
            .Where(c => c.UserId == query.UserId)
            .Where(c => !c.IsDeleted);

        commentsQuery = OrderBy(commentsQuery, query.Order);

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

    private static IQueryable<Comment> OrderBy(IQueryable<Comment> queryable, Order order) =>
        order switch
        {
            Order.CreatedAt => queryable.OrderBy(c => c.Id),
            Order.Title => queryable.OrderBy(c => c.Title),
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, "Unexpected order value."),
        };
}
