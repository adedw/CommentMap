using CommentMap.Mvc.Data;
using CommentMap.Mvc.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Mvc.Services;

public class ListCommentsService(ICommentMapDbContext dbContext) : IListCommentsService
{
    public async Task<List<CommentCardViewModel>> GetAllUserComments(Guid userId, CancellationToken cancellationToken = default)
    {
        var comments = await dbContext.Comments
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Id)
            .Select(c => new CommentCardViewModel(c.Id, new Location { Latitude = c.Location.Y, Longitude = c.Location.Y }, c.Title, c.Text, c.CreatedAt))
            .ToListAsync(cancellationToken);
        return comments;
    }
}
