using CommentMap.Mvc.Data;
using CommentMap.Mvc.Extensions;
using CommentMap.Mvc.Models;
using CommentMap.Mvc.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Mvc.Services;

public class ListCommentsService(ICommentMapDbContext dbContext) : IListCommentsService
{
    public async Task<List<CommentCardViewModel>> GetAllUserComments(GetAllCommentsDto dto, CancellationToken cancellationToken = default)
    {
        var comments = await dbContext.Comments
            .Where(c => c.UserId == dto.UserId)
            .Where(c => !c.IsDeleted)
            .OrderBy(dto.Order)
            .Select(c => new CommentCardViewModel(c.Id, new LocationViewModel { Longitude = c.Location.X, Latitude = c.Location.Y }, c.Title, c.Text, c.CreatedAt))
            .ToListAsync(cancellationToken);
        return comments;
    }
}
