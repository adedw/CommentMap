using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.Models;

namespace CommentMap.Mvc.Extensions;

public static class QueriableExtensions
{
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, Order order)
        where T : Comment
        => order switch
        {
            Order.CreatedAt => queryable.OrderBy(c => c.Id),
            Order.Title => queryable.OrderBy(c => c.Title),
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, "Unexpected order value."),
        };
}