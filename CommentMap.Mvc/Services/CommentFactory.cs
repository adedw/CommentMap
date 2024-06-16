using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Models;
using NetTopologySuite.Geometries;

namespace CommentMap.Mvc.Services;

public class CommentFactory(IIdGenerationService idGenerationService) : ICommentFactory
{
    public Comment CreateFrom(AddNewCommentDto addNewCommentDto)
    {
        var id = idGenerationService.GenerateId();
        var createdAt = DateTime.UtcNow;

        return new Comment
        {
            Id = id,
            UserId = addNewCommentDto.UserId,
            Location = new Point(addNewCommentDto.Longitude, addNewCommentDto.Latitude),
            Title = addNewCommentDto.Title,
            Text = addNewCommentDto.Text,
            CreatedAt = createdAt
        };
    }
}