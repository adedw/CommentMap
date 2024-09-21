using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Models;
using NetTopologySuite.Geometries;

namespace CommentMap.Mvc.Services;

public class CommentFactory(IIdGenerationService idGenerationService, IGuessCountryService guessCountryService) : ICommentFactory
{
    public async Task<Comment> CreateAsync(AddNewCommentDto addNewCommentDto, CancellationToken cancellationToken = default)
    {
        var point = new Point(addNewCommentDto.Longitude, addNewCommentDto.Latitude) { SRID = 3857 };
        var iso3Code = await guessCountryService.GetCountryCodeAsync(point, cancellationToken);
        var id = idGenerationService.GenerateId();
        var createdAt = DateTime.UtcNow;

        return new Comment
        {
            Id = id,
            UserId = addNewCommentDto.UserId,
            Location = point,
            Title = addNewCommentDto.Title,
            Text = addNewCommentDto.Text,
            CreatedAt = createdAt,
            ISO3CodeCountry = iso3Code,
        };
    }
}
