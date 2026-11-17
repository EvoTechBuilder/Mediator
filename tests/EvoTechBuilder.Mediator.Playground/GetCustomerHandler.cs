using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground;

/// <inheritdoc />
public sealed class GetCustomerHandler : IRequestHandler<GetCustomerQuery, Customer>
{
    /// <inheritdoc />
    public ValueTask<Customer> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = new Customer(
            request.Id,
            "Maria");

        return ValueTask.FromResult(customer);
    }
}
