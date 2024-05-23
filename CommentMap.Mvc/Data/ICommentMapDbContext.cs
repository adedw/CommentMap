using CommentMap.Mvc.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Mvc.Data;

public interface ICommentMapDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Comment> Comments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
