using EvoTechBuilder.Mediator.Abstractions.Contracts;
using EvoTechBuilder.Mediator.Playground.Notifications;

namespace EvoTechBuilder.Mediator.Playground.Handlers;

/// <summary>
/// Simulates sending a welcome email when a customer is created.
/// </summary>
public sealed class SendWelcomeEmailHandler
    : INotificationHandler<CustomerCreatedNotification>
{
    /// <inheritdoc />
    public async ValueTask Handle(
        CustomerCreatedNotification notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("[Email] Started");

        await Task.Delay(
            TimeSpan.FromSeconds(3),
            cancellationToken);
            
        Console.WriteLine("[Email] Failed");
            
        throw new InvalidOperationException("Failed to send welcome email.");
    }
}