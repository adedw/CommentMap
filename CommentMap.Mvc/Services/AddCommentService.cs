using CommentMap.Mvc.Data;
using CommentMap.Mvc.Models;

namespace CommentMap.Mvc.Services;

public class AddCommentService(ICommentMapDbContext dbContext, ICommentFactory commentFactory)
    : IAddCommentService
{
    public Task AddAsync(AddNewCommentDto addNewCommentDto, CancellationToken cancellationToken)
    {
        var comment = commentFactory.CreateFrom(addNewCommentDto);

        dbContext.Comments.Add(comment);

        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
