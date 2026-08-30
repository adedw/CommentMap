using CommentMap.Application.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommentMap.Infrastructure.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.ISO3Code);
        builder.Property(c => c.ISO3Code).HasMaxLength(3).HasColumnType("char");
        builder.Property(c => c.ISO2Code).HasMaxLength(3).HasColumnType("char");

        builder.Property(c => c.Shape).HasColumnType("geometry (multipolygon, 3857)");
        builder.HasIndex(c => c.Shape).HasMethod("gist");

        builder.Property(c => c.Name).HasMaxLength(50);
        builder.Property(c => c.LocalName).HasMaxLength(255);
    }
}
