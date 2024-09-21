using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Models;

namespace CommentMap.Mvc.Services;

public interface ICommentFactory
{
    Task<Comment> CreateAsync(AddNewCommentDto addNewCommentDto, CancellationToken cancellationToken = default);
}