namespace CommentMap.Application.Models;

public record CountryDto(
    string ISO3Code,
    string ISO2Code,
    string Name,
    string RegionName,
    string SubregionName);
