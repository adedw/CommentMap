using CommentMap.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace CommentMap.Mvc.Services;

public class GuessCountryService(ICommentMapDbContext dbContext) : IGuessCountryService
{
    public Task<string?> GetCountryCodeAsync(Point point, CancellationToken cancellationToken = default)
    {
        return dbContext.Countries
            .Where(c => c.Boundaries.Intersects(point))
            .Select(c => c.ISO3Code)
            .FirstOrDefaultAsync(cancellationToken);
    }
}