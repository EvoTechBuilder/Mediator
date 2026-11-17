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
        
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
        
        return handler.Handle(request, cancellationToken);
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
