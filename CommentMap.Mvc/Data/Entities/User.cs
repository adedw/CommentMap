using Microsoft.AspNetCore.Identity;

namespace CommentMap.Mvc.Data.Entities;

public class User : IdentityUser<Guid>
{
    public HashSet<Comment>? Comments { get; set; }
}
