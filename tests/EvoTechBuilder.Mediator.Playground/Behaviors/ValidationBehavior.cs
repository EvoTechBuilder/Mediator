using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground.Behaviors;

/// <summary>
/// Simulates request validation before executing the next pipeline component.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Validation] Validating {typeof(TRequest).Name}");

        return await next(cancellationToken);
    }
}
