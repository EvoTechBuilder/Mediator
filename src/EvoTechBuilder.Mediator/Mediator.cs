using System.Runtime.ExceptionServices;

using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Abstractions.Enums;
using EvoTechBuilder.Mediator.Abstractions.Options;
using EvoTechBuilder.Mediator.Internal;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EvoTechBuilder.Mediator;

/// <summary>
/// Provides the implementation for sending requests
/// and publishing notifications.
/// </summary>
public class Mediator(
    IServiceProvider serviceProvider,
    IOptions<MediatorOptions> options) : ISender, IPublisher
{
    private readonly MediatorOptions _options = options.Value;

    /// <inheritdoc />
    public async ValueTask Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);
        cancellationToken.ThrowIfCancellationRequested();

        var handlers = serviceProvider
            .GetServices<INotificationHandler<TNotification>>()
            .ToArray();

        if (handlers.Length == 0)
        {
            throw new InvalidOperationException(
                $"No notification handlers registered for '{typeof(TNotification).FullName}'.");
        }

        if (_options.NotificationExecution == NotificationExecution.Sequential)
        {
            await PublishSequentially(
                handlers,
                notification,
                cancellationToken);

            return;
        }

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
            if (task.Exception is { } aggregateException)
            {
                ExceptionDispatchInfo
                    .Capture(aggregateException)
                    .Throw();
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
    private static async ValueTask PublishSequentially<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        List<Exception> exceptions = [];

        foreach (var handler in handlers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await handler.Handle(notification, cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }

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
