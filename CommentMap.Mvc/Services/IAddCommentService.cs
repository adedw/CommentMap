using CommentMap.Mvc.Models;

namespace CommentMap.Mvc.Services;

public interface IAddCommentService
{
    Task AddAsync(AddNewCommentDto addNewCommentDto, CancellationToken cancellationToken);
}