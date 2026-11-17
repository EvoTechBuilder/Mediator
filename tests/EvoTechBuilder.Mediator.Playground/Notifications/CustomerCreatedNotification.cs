using EvoTechBuilder.Mediator.Abstractions.Contracts;

namespace EvoTechBuilder.Mediator.Playground.Notifications;

/// <summary>
/// Represents the event raised when a customer is created.
/// </summary>
/// <param name="CustomerId">The identifier of the created customer.</param>
public sealed record CustomerCreatedNotification(Guid CustomerId)
    : INotification;
