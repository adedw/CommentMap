using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Data.Entities.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Mvc.Data;

public class CommentMapDbContext(DbContextOptions<CommentMapDbContext> options) : IdentityDbContext<User, Role, Guid>(options), ICommentMapDbContext
{
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new CommentConfiguration());
    }
}
