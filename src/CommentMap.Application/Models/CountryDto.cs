namespace CommentMap.Application.Models;

public record CountryDTO(
    string ISO3Code,
    string ISO2Code,
    string Name,
    string LocalName);
