using EvoTechBuilder.Mediator.Abstractions.Contracts;

using Microsoft.Extensions.DependencyInjection;

namespace EvoTechBuilder.Mediator.Internal;

/// <summary>
/// Provides the implementation for resolving and executing
/// a request handler.
/// </summary>
/// <typeparam name="TRequest">
/// The concrete request type.
/// </typeparam>
/// <typeparam name="TResponse">
/// The response type produced by the handler.
/// </typeparam>
internal sealed class RequestHandlerWrapper<TRequest, TResponse>
    : IRequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public ValueTask<TResponse> Handle(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var handler = serviceProvider
            .GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        return handler.Handle(
            (TRequest)request,
            cancellationToken);
    }
}