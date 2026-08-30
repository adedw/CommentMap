using NetTopologySuite.Geometries;

namespace CommentMap.Application.Entities;

public class Country
{
    public required string ISO3Code { get; set; }
    public required string ISO2Code { get; set; }

    public required string Name { get; set; }
    public required string LocalName { get; set; }

    public required MultiPolygon Shape { get; set; }

    public HashSet<Comment>? Comments { get; set; }
}
