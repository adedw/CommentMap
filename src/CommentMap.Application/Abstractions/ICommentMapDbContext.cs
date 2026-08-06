using CommentMap.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommentMap.Application.Abstractions;

public interface ICommentMapDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Country> Countries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
