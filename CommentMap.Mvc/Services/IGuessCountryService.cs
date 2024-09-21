using NetTopologySuite.Geometries;

namespace CommentMap.Mvc.Services;

public interface IGuessCountryService
{
    Task<string?> GetCountryCodeAsync(Point point, CancellationToken cancellationToken = default);
}