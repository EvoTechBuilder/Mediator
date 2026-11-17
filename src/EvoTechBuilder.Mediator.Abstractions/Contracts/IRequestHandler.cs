namespace EvoTechBuilder.Mediator.Abstractions.Contracts;

/// <summary>
/// Defines a handler responsible for processing a request and producing a response.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being handled.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response produced by the handler.
/// </typeparam>
public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the specified request asynchronously.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation
    /// and containing the response.
    /// </returns>
    ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellation);
}

/// <summary>
/// Defines a handler responsible for processing a request that does not produce a response.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request to be handled.
/// </typeparam>
public interface IRequestHandler<TRequest> where TRequest : IRequest {
    /// <summary>
    /// Handles the specified request asynchronously and returns a <see cref="ValueTask"/> representing the operation.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellation">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Handle(TRequest request, CancellationToken cancellation);
}