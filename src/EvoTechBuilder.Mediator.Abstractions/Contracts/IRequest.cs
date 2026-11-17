namespace EvoTechBuilder.Mediator.Abstractions.Contracts;

/// <summary>
/// Represents a request that can be dispatched through the mediator pipeline.
/// </summary>
public interface IRequest {}

/// <summary>
/// Represents a request that can be dispatched through the mediator pipeline and expects a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response expected from the request.
/// </typeparam>
public interface IRequest<TResponse> : IRequest
{
}
