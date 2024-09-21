using CommentMap.Mvc.Data;
using CommentMap.Mvc.Models;

namespace CommentMap.Mvc.Services;

public class AddCommentService(ICommentMapDbContext dbContext, ICommentFactory commentFactory)
    : IAddCommentService
{
    public async Task AddAsync(AddNewCommentDto addNewCommentDto, CancellationToken cancellationToken)
    {
        var comment = await commentFactory.CreateAsync(addNewCommentDto, cancellationToken);

        dbContext.Comments.Add(comment);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
