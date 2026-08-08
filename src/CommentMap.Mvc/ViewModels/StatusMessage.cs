namespace CommentMap.Mvc.ViewModels;

public enum StatusType { Info, Success, Error}

/// <summary>
/// Represents a status message shown to the user via the StatusAlert view component.
/// </summary>
public sealed record StatusMessage(StatusType Type, string Message)
{
    public static StatusMessage Info(string message) => new(StatusType.Info, message);
    public static StatusMessage Success(string message) => new(StatusType.Success, message);
    public static StatusMessage Error(string message) => new(StatusType.Error, message);
}
