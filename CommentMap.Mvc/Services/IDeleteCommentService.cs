
namespace CommentMap.Mvc.Services
{
    public interface IDeleteCommentService
    {
        Task DeleteCommentAsync(Guid id, CancellationToken cancellationToken = default);
    }
}