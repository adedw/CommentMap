using CommentMap.Mvc.Models;
using CommentMap.Mvc.ViewModels;

namespace CommentMap.Mvc.Services;

public interface IListCommentsService
{
    Task<List<CommentCardViewModel>> GetAllUserComments(GetAllCommentsDto dto, CancellationToken cancellationToken = default);
}