using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground.Behaviors;

/// <inheritdoc />
public sealed class LoggingBehavior<TRequest>
    : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    /// <inheritdoc />
    public async ValueTask Handle(
        TRequest request,
        RequestHandlerDelegate next,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine($"[Before] Handling {typeof(TRequest).Name}");

        await next();

        Console.WriteLine($"[After] Handled {typeof(TRequest).Name}");
    }
}

/// <inheritdoc />
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine($"[Before] Handling {typeof(TRequest).Name}");

        var response = await next();

        Console.WriteLine($"[After] Handled {typeof(TRequest).Name}");

        return response;
    }
}
