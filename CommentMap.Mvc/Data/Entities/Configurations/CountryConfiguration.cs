using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommentMap.Mvc.Data.Entities.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.Property(c => c.Boundaries).HasColumnType("geometry (multipolygon, 3857)");
        builder.HasIndex(c => c.Boundaries).HasMethod("gist");

        builder.HasKey(c => c.ISO3Code);
        builder.Property(c => c.ISO3Code).HasMaxLength(3).HasColumnType("char");
        builder.Property(c => c.ISO2Code).HasMaxLength(2).HasColumnType("char");

        builder.Property(c => c.Name).HasMaxLength(60);
        builder.Property(c => c.RegionName).HasMaxLength(10);
        builder.Property(c => c.SubregionName).HasMaxLength(40);
    }
}
