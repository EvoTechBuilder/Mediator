using System.Collections.Concurrent;

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
    private static readonly ConcurrentDictionary<Type, Lazy<object>> WrapperCache = new();
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

        await PublishInParallel(handlers, notification, _options.MaxConcurrentHandlers, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var wrapperType = typeof(RequestHandlerWrapper<>)
            .MakeGenericType(typeof(TRequest));

        var wrapper = (IRequestHandlerWrapper)GetWrapper(wrapperType);

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
            GetWrapper(wrapperType);

        return wrapper.Handle(
            request,
            serviceProvider,
            cancellationToken);
    }

    #region Private Methods
    private static object GetWrapper(Type wrapperType)
    {
        return WrapperCache.GetOrAdd(
            wrapperType,
            static type => new Lazy<object>(
                () => Activator.CreateInstance(type, nonPublic: true)!,
                LazyThreadSafetyMode.ExecutionAndPublication))
            .Value;
    }

    private static async ValueTask PublishInParallel<TNotification>(
        INotificationHandler<TNotification>[] handlers,
        TNotification notification,
        uint maxConcurrentHandlers,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        var workerCount = maxConcurrentHandlers == 0
            ? handlers.Length
            : (int)Math.Min(maxConcurrentHandlers, (uint)handlers.Length);

        var exceptions = new ConcurrentQueue<Exception>();
        var nextIndex = -1;

        var workers = new Task[workerCount];

        for (var index = 0; index < workerCount; index++)
        {
            workers[index] = ProcessHandlers();
        }

        await Task.WhenAll(workers);

        if (!exceptions.IsEmpty)
        {
            throw new AggregateException(exceptions);
        }

        cancellationToken.ThrowIfCancellationRequested();

        async Task ProcessHandlers()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var index = Interlocked.Increment(ref nextIndex);

                if (index >= handlers.Length)
                {
                    return;
                }

                try
                {
                    await handlers[index].Handle(notification, cancellationToken);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    exceptions.Enqueue(exception);
                }
            }
        }
    }

    private static async ValueTask PublishSequentially<TNotification>(
        IEnumerable<INotificationHandler<TNotification>> handlers,
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        List<Exception> exceptions = [];

        foreach (var handler in handlers)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await handler.Handle(notification, cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                break;
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

        cancellationToken.ThrowIfCancellationRequested();
    }

    #endregion
}
