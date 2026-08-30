using CommentMap.Application.Abstractions;
using CommentMap.Application.Models;

using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Countries.Queries;

public record GetCountryQuery(string ISO3Code) : IQuery<CountryDTO?>;

public sealed class GetCountryHandler(ICommentMapDbContext db) : IQueryHandler<GetCountryQuery, CountryDTO?>
{
    public Task<CountryDTO?> Handle(GetCountryQuery query, CancellationToken cancellationToken)
    {
        return db.Countries
            .AsNoTracking()
            .Where(c => c.ISO3Code.ToLower().Equals(query.ISO3Code.ToLower()))
            .Select(c => new CountryDTO(
                c.ISO3Code,
                c.ISO2Code,
                c.Name,
                c.LocalName))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
