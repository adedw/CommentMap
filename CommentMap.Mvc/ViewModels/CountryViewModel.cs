namespace CommentMap.Mvc.ViewModels;

public class CountryViewModel
{
    public required string ISO3Code { get; set; }
    public required string ISO2Code { get; set; }
    public required string Name { get; set; }
    public required string RegionName { get; set; }
    public required string SubregionName { get; set; }
}

