using CommentMap.Mvc.Data;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Mvc.Services;

public class DeleteCommentService(ICommentMapDbContext dbContext) : IDeleteCommentService
{
    public Task DeleteCommentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Comments
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(q => q.IsDeleted, true), cancellationToken);
    }
}
