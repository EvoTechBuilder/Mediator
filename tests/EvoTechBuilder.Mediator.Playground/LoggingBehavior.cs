using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Abstractions.Contracts.Delegates;

namespace EvoTechBuilder.Mediator.Playground;

/// <inheritdoc />
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Before] Handling {typeof(TRequest).Name}");

        var response = await next();

        Console.WriteLine($"[After] Handled {typeof(TRequest).Name}");

        return response;
    }
}
