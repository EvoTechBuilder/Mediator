using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Playground.Request;

namespace EvoTechBuilder.Mediator.Playground.Handlers;

/// <summary>
/// Handles customer creation commands.
/// </summary>
public sealed class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand>
{
    /// <inheritdoc />
    public ValueTask Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Creating customer: {request.Name}");

        return ValueTask.CompletedTask;
    }
}
