using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Models;

namespace CommentMap.Mvc.Services;

public interface ICommentFactory
{
    Comment CreateFrom(AddNewCommentDto addNewCommentDto);
}