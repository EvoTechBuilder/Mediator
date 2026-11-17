namespace EvoTechBuilder.Mediator.Abstractions.Contracts.Delegates;

/// <summary>
/// Represents the next component in the request processing pipeline.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response produced by the request handler.
/// </typeparam>
/// <returns>
/// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation
/// and containing the response.
/// </returns>
public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Represents the next component in the request processing pipeline.
/// </summary>
/// <returns>
/// A <see cref="ValueTask"/> representing the asynchronous operation.
/// </returns>
public delegate ValueTask RequestHandlerDelegate();