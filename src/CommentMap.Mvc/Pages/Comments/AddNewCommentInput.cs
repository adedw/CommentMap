using System.ComponentModel.DataAnnotations;

namespace CommentMap.Mvc.Pages.Comments;

public class AddNewCommentInput
{
    [Required]
    [StringLength(100)]
    public string? Title { get; init; }

    [Required]
    [StringLength(250)]
    public string? Text { get; init; }

    [Required]
    [Range(-20037508.3427892, 20037508.3427892)]
    public double? Longitude { get; init; }

    [Required]
    [Range(-20037508.3427892, 20037508.3427892)]
    public double? Latitude { get; init; }
}
