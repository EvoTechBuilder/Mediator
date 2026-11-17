using EvoTechBuilder.Mediator.Abstractions.Contracts.Delegates;

namespace EvoTechBuilder.Mediator.Abstractions.Contracts;

/// <summary>
/// Defines a behavior that can execute logic before and after
/// the processing of a request.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being processed.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response produced by the request handler.
/// </typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Executes the behavior and delegates request processing
    /// to the next component in the pipeline.
    /// </summary>
    /// <param name="request">
    /// The request being processed.
    /// </param>
    /// <param name="next">
    /// A delegate that invokes the next behavior or request handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation
    /// and containing the response.
    /// </returns>
    ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);

}

/// <summary>
/// Defines a behavior that can execute logic before and after
/// the processing of a request that does not produce a response.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being processed.
/// </typeparam>
public interface IPipelineBehavior<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Executes the behavior and delegates request processing
    /// to the next component in the pipeline.
    /// </summary>
    /// <param name="request">
    /// The request being processed.
    /// </param>
    /// <param name="next">
    /// A delegate that invokes the next behavior or request handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    ValueTask Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken);
}
