using CommentMap.Mvc.DataAnnotations;
using CommentMap.Mvc.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CommentMap.Mvc.Models;

public class AddNewCommentInput
{
    [Required]
    [StringLength(100)]
    public string? Title { get; init; }

    [Required]
    [StringLength(250)]
    public string? Text { get; init; }
    
    [LocationRequired]
    public required Location Location { get; init; }
}
