namespace CommentMap.Mvc.Services;

public interface IConfirmDeleteService
{
    Task<string?> GetCommentTitleAsync(Guid id, CancellationToken cancellationToken);
}
