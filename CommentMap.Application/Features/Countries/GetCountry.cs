using CommentMap.Application.Abstractions;
using CommentMap.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Features.Countries;

public record GetCountry(string ISO3Code);

public static class GetCountryHandler
{
    public static Task<CountryDto?> Handle(
        GetCountry query,
        ICommentMapDbContext db,
        CancellationToken cancellationToken)
    {
        return db.Countries
            .Where(c => c.ISO3Code.ToLower().Equals(query.ISO3Code.ToLower()))
            .Select(c => new CountryDto(
                c.ISO3Code,
                c.ISO2Code,
                c.Name,
                c.RegionName,
                c.SubregionName))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
