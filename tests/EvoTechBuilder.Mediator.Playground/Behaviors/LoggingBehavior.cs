using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground.Behaviors;

/// <inheritdoc />
public sealed class LoggingBehavior<TRequest>
    : IPipelineBehavior<TRequest>
    where TRequest : IRequest
{
    /// <inheritdoc />
    public async ValueTask Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Before] Handling {typeof(TRequest).Name}");

        await next();

        Console.WriteLine($"[After] Handled {typeof(TRequest).Name}");
    }
}
