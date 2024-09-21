using NetTopologySuite.Geometries;

namespace CommentMap.Mvc.Data.Entities;

public class Country
{
    public required MultiPolygon Boundaries { get; set; }
    public required string ISO3Code { get; set; }
    public required string ISO2Code { get; set; }
    public required string Name { get; set; }
    public short RegionCode { get; set; }
    public required string RegionName { get; set; }
    public short SubregionCode { get; set; }
    public required string SubregionName { get; set; }

    public HashSet<Comment>? Comments { get; set; }
}
