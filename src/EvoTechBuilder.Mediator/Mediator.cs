using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Internal;

using Microsoft.Extensions.DependencyInjection;

namespace EvoTechBuilder.Mediator;

/// <summary>
/// Provides the implementation for sending requests
/// and publishing notifications.
/// </summary>
public class Mediator(IServiceProvider serviceProvider) : ISender, IPublisher
{
    /// <inheritdoc />
    public async ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);
        cancellationToken.ThrowIfCancellationRequested();

        var handlers = serviceProvider
            .GetServices<INotificationHandler<TNotification>>()
            .ToArray();

        var tasks = handlers
        .Select(handler => InvokeHandler(
            handler,
            notification,
            cancellationToken))
        .ToArray();

        var task = Task.WhenAll(tasks);

        try
        {
            await task;
        }
        catch
        {
            if (task.Exception is { } aggreateException)
            {
                throw aggreateException;
            }

            throw;
        }
    }

    /// <inheritdoc />
    public ValueTask Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var wrapperType = typeof(RequestHandlerWrapper<>)
            .MakeGenericType(typeof(TRequest));

        var wrapper = (IRequestHandlerWrapper)
            Activator.CreateInstance(wrapperType, nonPublic: true)!;

        return wrapper.Handle(
            request,
            serviceProvider,
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var wrapperType = typeof(RequestHandlerWrapper<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var wrapper = (IRequestHandlerWrapper<TResponse>)
            Activator.CreateInstance(wrapperType, nonPublic: true)!;

        return wrapper.Handle(
            request,
            serviceProvider,
            cancellationToken);
    }

    #region Private Methods
    private static async Task InvokeHandler<TNotification>(
        INotificationHandler<TNotification> handler,
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        await handler.Handle(notification, cancellationToken);
    }
    #endregion
}
