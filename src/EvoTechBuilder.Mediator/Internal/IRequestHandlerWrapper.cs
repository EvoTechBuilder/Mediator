namespace EvoTechBuilder.Mediator.Internal;

/// <summary>
/// Defines an internal mechanism for dispatching requests
/// to their corresponding handlers.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response produced by the request handler.
/// </typeparam>
internal interface IRequestHandlerWrapper<TResponse>
{
    /// <summary>
    /// Resolves and executes the handler for the specified request.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="serviceProvider">
    /// The service provider used to resolve the handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> containing the handler response.
    /// </returns>
    ValueTask<TResponse> Handle(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

/// <summary>
/// Defines a wrapper for requests that do not produce a response.
/// </summary>
internal interface IRequestHandlerWrapper
{
    ValueTask Handle(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}