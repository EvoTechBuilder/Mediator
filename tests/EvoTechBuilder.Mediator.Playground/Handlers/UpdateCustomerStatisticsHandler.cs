using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Playground.Notifications;

namespace EvoTechBuilder.Mediator.Playground.Handlers;

/// <summary>
/// Simulates updating customer statistics when a customer is created.
/// </summary>
public sealed class UpdateCustomerStatisticsHandler
    : INotificationHandler<CustomerCreatedNotification>
{
    /// <inheritdoc />
    public async ValueTask Handle(
        CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("[Statistics] Started");

        await Task.Delay(
            TimeSpan.FromSeconds(2),
            cancellationToken);
            
        Console.WriteLine("[Statistics] Failed");

        throw new InvalidOperationException(
            "Failed to update customer statistics.");
    }
}