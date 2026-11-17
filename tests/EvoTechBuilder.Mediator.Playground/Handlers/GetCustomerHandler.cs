using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground.Handlers;

/// <inheritdoc />
public sealed class GetCustomerHandler : IRequestHandler<GetCustomerQuery, Customer>
{
    /// <inheritdoc />
    public async ValueTask<Customer> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine("[Handler] Started");
        
        await Task.Delay(
        TimeSpan.FromSeconds(5),
        cancellationToken);

        Console.WriteLine("[Handler] Completed");

        return new Customer(request.Id, "Maria");
    }
}
