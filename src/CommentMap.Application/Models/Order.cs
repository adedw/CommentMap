using System.ComponentModel.DataAnnotations;

namespace CommentMap.Application.Models;

public enum Order
{
    [Display(Name = "Created At")]
    CreatedAt,

    [Display(Name = "Title")]
    Title
}
