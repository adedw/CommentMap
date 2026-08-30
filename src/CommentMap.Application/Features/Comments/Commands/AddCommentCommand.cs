using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;

using Microsoft.EntityFrameworkCore;

using NetTopologySuite.Geometries;

namespace CommentMap.Application.Features.Comments.Commands;

public record AddCommentCommand(Guid UserId, string Title, string Text, double Longitude, double Latitude) : ICommand;

public sealed class AddCommentHandler(ICommentMapDbContext db) : ICommandHandler<AddCommentCommand>
{
    public async Task Handle(AddCommentCommand command, CancellationToken cancellationToken)
    {
        var point = new Point(command.Longitude, command.Latitude) { SRID = 3857 };
        var iso3Code = await db.Countries
            .Where(c => c.Shape.Intersects(point))
            .Select(c => c.ISO3Code)
            .FirstOrDefaultAsync(cancellationToken);

        var comment = new Comment
        {
            Id = Guid.CreateVersion7(),
            UserId = command.UserId,
            Location = point,
            Title = command.Title,
            Text = command.Text,
            CreatedAt = DateTime.UtcNow,
            ISO3CodeCountry = iso3Code,
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);
    }
}
