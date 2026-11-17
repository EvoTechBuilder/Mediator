using EvoTechBuilder.Mediator.Abstractions.Enums;

namespace EvoTechBuilder.Mediator.Abstractions.Options;

/// <summary>
/// Defines the configuration options for the Mediator implementation.
/// </summary>
public sealed class MediatorOptions
{
    /// <summary>
    /// Gets or sets the notification handler execution strategy.
    /// Defaults to parallel execution.
    /// </summary>
    public NotificationExecution NotificationExecution { get; set; }
        = NotificationExecution.Parallel;

    /// <summary>
    /// Gets or sets the maximum number of notification handlers
    /// that can execute concurrently per publication.
    /// A value of zero allows unlimited concurrency.
    /// </summary>
    public uint MaxConcurrentHandlers { get; set; }
}