using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Abstractions.Contracts.Delegates;

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
        var typedRequest = (TRequest)request;

        var handler = serviceProvider
            .GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        var behaviors = serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .ToArray();

        RequestHandlerDelegate<TResponse> next = () => handler.Handle(typedRequest, cancellationToken);
        
        for (var index = behaviors.Length -1; index >= 0; index--)
        {
            var currentBehavior = behaviors[index];
            var currentNext = next;
            
            next = () => currentBehavior.Handle(
                typedRequest,
                currentNext,
                cancellationToken
            );
        }
        
        return next();
    }
}