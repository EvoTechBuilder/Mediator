using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground;

/// <inheritdoc />
public sealed record GetCustomerQuery(Guid Id) : IRequest<Customer>;
