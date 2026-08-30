using CommentMap.Application.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Comments.Commands;

public record DeleteCommentCommand(Guid Id) : ICommand;

public sealed class DeleteCommentHandler(ICommentMapDbContext db) : ICommandHandler<DeleteCommentCommand>
{
    public Task Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        return db.Comments
            .Where(c => c.Id == command.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(q => q.IsDeleted, true), cancellationToken);
    }
}
