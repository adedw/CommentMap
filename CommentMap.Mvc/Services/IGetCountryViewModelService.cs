using CommentMap.Mvc.ViewModels;

namespace CommentMap.Mvc.Services;

public interface IGetCountryViewModelService
{
    Task<CountryViewModel?> GetCountryViewModelAsync(string iso3code, CancellationToken cancellationToken = default);
}