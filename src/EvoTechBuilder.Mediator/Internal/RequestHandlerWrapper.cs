using EvoTechBuilder.Mediator.Abstractions.Contracts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    public ValueTask<TResponse> Handle(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;

        var handlers = serviceProvider
            .GetServices<IHandler<TRequest, TResponse>>()
            .ToArray();

        if (handlers.Length == 0)
        {
            throw new InvalidOperationException(
                $"No request handler registered for '{typeof(TRequest).FullName}'.");
        }

        if (handlers.Length > 1)
        {
            var logger = serviceProvider.GetService<ILogger<Mediator>>();

            logger?.LogWarning(
                "Multiple handlers were registered for request '{RequestType}'. " +
                "Only one handler is allowed per request. " +
                "Registered handlers: {HandlerTypes}. " +
                "The last registered handler will be used.",
                typeof(TRequest).Name,
                string.Join(", ", handlers.Select(handler =>
                    handler.GetType().Name)));
        }

        var handler = handlers[^1];

        var behaviors = serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .ToArray();

        RequestHandlerDelegate<TResponse> next =
            token => handler.Handle(typedRequest, token);

        for (var index = behaviors.Length - 1; index >= 0; index--)
        {
            var currentBehavior = behaviors[index];
            var currentNext = next;

            next = token => currentBehavior.Handle(
                typedRequest,
                currentNext,
                token);
        }

        return next(cancellationToken);
    }
}

/// <summary>
/// Wraps the execution of a request handler with its registered pipeline behaviors.
/// </summary>
/// <typeparam name="TRequest">
/// The concrete request type.
/// </typeparam>
internal sealed class RequestHandlerWrapper<TRequest>
    : IRequestHandlerWrapper
    where TRequest : IRequest
{
    /// <inheritdoc />
    public ValueTask Handle(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;

        var handlers = serviceProvider
            .GetServices<IHandler<TRequest>>()
            .ToArray();

        if (handlers.Length == 0)
        {
            throw new InvalidOperationException(
                $"No request handler registered for '{typeof(TRequest).FullName}'.");
        }

        if (handlers.Length > 1)
        {
            var logger = serviceProvider.GetService<ILogger<Mediator>>();

            logger?.LogWarning(
                "Multiple handlers were registered for request '{RequestType}'. " +
                "Only one handler is allowed per request. " +
                "Registered handlers: {HandlerTypes}. " +
                "The last registered handler will be used.",
                typeof(TRequest).Name,
                string.Join(", ", handlers.Select(handler =>
                    handler.GetType().Name)));
        }

        var handler = handlers[^1];

        var behaviors = serviceProvider
            .GetServices<IPipelineBehavior<TRequest>>()
            .ToArray();

        RequestHandlerDelegate next =
            token => handler.Handle(typedRequest, token);

        for (var index = behaviors.Length - 1; index >= 0; index--)
        {
            var currentBehavior = behaviors[index];
            var currentNext = next;

            next = token => currentBehavior.Handle(
                typedRequest,
                currentNext,
                token);
        }

        return next(cancellationToken);
    }
}
