using CommentMap.Mvc.Services;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CommentMap.Mvc.Pages.Countries;

public class IndexModel(IGetCountryViewModelService getCountryViewModelService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessage = "ISO 3166-1 alpha-3 code required.")]
    [StringLength(maximumLength: 3, MinimumLength = 3, ErrorMessage = "ISO 3166-1 alpha-3 code must contains 3 letters.")]
    public string? ISO3Code { get; set; }
    public CountryViewModel? Country { get; private set; }

    public async Task<PageResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Country = await getCountryViewModelService.GetCountryViewModelAsync(ISO3Code!, cancellationToken);
        return Page();
    }
}
