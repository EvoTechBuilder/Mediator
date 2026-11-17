using System.Diagnostics.CodeAnalysis;

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
    public ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc />
    public ValueTask Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

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
        
        var wrapperType = typeof(RequestHandlerWrapper<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var wrapper = (IRequestHandlerWrapper<TResponse>)
            Activator.CreateInstance(wrapperType, nonPublic: true)!;

        return wrapper.Handle(
            request,
            serviceProvider,
            cancellationToken);
    }
}
