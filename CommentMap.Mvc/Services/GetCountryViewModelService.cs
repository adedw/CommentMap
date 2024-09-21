using CommentMap.Mvc.Data;
using CommentMap.Mvc.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Mvc.Services;

public class GetCountryViewModelService(CommentMapDbContext dbContext) : IGetCountryViewModelService
{
    public Task<CountryViewModel?> GetCountryViewModelAsync(string iso3code, CancellationToken cancellationToken = default)
    {
        return dbContext.Countries
            .Where(c => c.ISO3Code.ToLower().Equals(iso3code.ToLower()))
            .Select(c => new CountryViewModel
            {
                ISO3Code = c.ISO3Code,
                ISO2Code = c.ISO2Code,
                Name = c.Name,
                RegionName = c.RegionName,
                SubregionName = c.SubregionName
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}

