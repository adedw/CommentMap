using CommentMap.Application.Features.Countries;
using CommentMap.Application.Models;
using CommentMap.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Wolverine;

namespace CommentMap.Mvc.Pages.Countries;

public class IndexModel(IMessageBus bus) : PageModel
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

        var dto = await bus.InvokeAsync<CountryDto?>(new GetCountry(ISO3Code!), cancellationToken);
        if (dto is not null)
        {
            Country = new CountryViewModel
            {
                ISO3Code = dto.ISO3Code,
                ISO2Code = dto.ISO2Code,
                Name = dto.Name,
                RegionName = dto.RegionName,
                SubregionName = dto.SubregionName,
            };
        }

        return Page();
    }
}
