namespace CommentMap.Application.Abstractions;

/// <summary>
/// Marker for commands handled in-process by <see cref="ICommandHandler{TCommand}"/> / <see cref="ICommandHandler{TCommand, TResult}"/>.
/// </summary>
public interface ICommand;

/// <summary>
/// Marker for commands that produce a result of type <typeparamref name="TResult"/>.
/// </summary>
public interface ICommand<TResult> : ICommand;

/// <summary>
/// Marker for queries handled in-process by <see cref="IQueryHandler{TQuery, TResult}"/>.
/// </summary>
public interface IQuery<TResult>;
