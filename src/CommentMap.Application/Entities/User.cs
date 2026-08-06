using Microsoft.AspNetCore.Identity;

namespace CommentMap.Application.Entities;

public class User : IdentityUser<Guid>
{
    public HashSet<Comment>? Comments { get; set; }
}
