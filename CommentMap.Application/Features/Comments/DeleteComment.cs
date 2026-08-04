using CommentMap.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Comments;

public record DeleteComment(Guid Id);

public static class DeleteCommentHandler
{
    public static Task Handle(DeleteComment command, ICommentMapDbContext db, CancellationToken cancellationToken)
    {
        return db.Comments
            .Where(c => c.Id == command.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(q => q.IsDeleted, true), cancellationToken);
    }
}
