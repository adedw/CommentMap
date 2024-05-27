using NetTopologySuite.Geometries;

namespace CommentMap.Mvc.Data.Entities;

public class Comment : IEquatable<Comment>
{
    public required Guid Id { get; init; }

    public required User User { get; init; }
    public Guid UserId { get; init; }

    public required Point Location { get; set; }

    public required string Title { get; set; }
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; init; }

    public bool Equals(Comment? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Comment);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
