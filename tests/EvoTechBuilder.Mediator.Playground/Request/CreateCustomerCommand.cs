using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground.Request;

/// <summary>
/// Represents a command to create a customer.
/// </summary>
/// <param name="Name">The name of the customer to create.</param>
public sealed record CreateCustomerCommand(string Name) : IRequest;
