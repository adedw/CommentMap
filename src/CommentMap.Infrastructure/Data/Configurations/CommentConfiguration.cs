using CommentMap.Application.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommentMap.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.Location).HasColumnType("geometry (point, 3857)");
        builder.HasIndex(c => c.Location).HasMethod("gist");

        builder.Property(c => c.Title).HasMaxLength(100);
        builder.Property(c => c.Text).HasMaxLength(250);
        builder.Property(c => c.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(c => c.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(c => c.Title);

        builder.HasIndex(c => c.IsDeleted)
            .HasFilter("\"IsDeleted\" = false")
            .HasDatabaseName("IX_Comments_IsDeleted_Filtered");

        builder.HasOne(c => c.Country)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.ISO3CodeCountry)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
