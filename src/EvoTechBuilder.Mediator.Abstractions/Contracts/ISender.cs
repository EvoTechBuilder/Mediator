namespace EvoTechBuilder.Mediator.Abstractions.Contracts;

/// <summary>
/// Defines a component responsible for dispatching requests to their corresponding handlers.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Sends a request that does not produce a response to its corresponding handler.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The type of the request being sent.
    /// </typeparam>
    /// <param name="request">The request to dispatch.</param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous dispatch operation.
    /// </returns>
    ValueTask Send<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest;

    /// <summary>
    /// Sends a request and returns the response produced by its corresponding handler.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of the response expected from the handler.
    /// </typeparam>
    /// <param name="request">The request to dispatch.</param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous dispatch operation
    /// and containing the handler response.
    /// </returns>
    ValueTask<TResponse> Send<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}