using CommentMap.Mvc.Data;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Mvc.Services;

public class ConfirmDeleteService(ICommentMapDbContext dbContext) : IConfirmDeleteService
{
    public async Task<string?> GetCommentTitleAsync(Guid id, CancellationToken cancellationToken)
    {
        var title = await dbContext.Comments
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => c.Title)
            .FirstOrDefaultAsync(cancellationToken);
        return title;
    }
}
