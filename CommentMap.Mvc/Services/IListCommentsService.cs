using CommentMap.Mvc.ViewModels;

namespace CommentMap.Mvc.Services
{
    public interface IListCommentsService
    {
        Task<List<CommentCardViewModel>> GetAllUserComments(Guid userId, CancellationToken cancellationToken = default);
    }
}