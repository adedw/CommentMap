using Mjml.Net;

namespace CommentMap.EmailSender.Exceptions;


public class MjmlValidationException(ValidationErrors errors)
    : Exception("Exception during rendering mjml template.")
{
    public ValidationErrors Errors { get; } = errors;
}