namespace EvoTechBuilder.Mediator.Abstractions.Enums;

/// <summary>
/// Defines how notification handlers are executed
/// when a notification is published.
/// </summary>
public enum NotificationExecution
{
    /// <summary>
    /// Executes all notification handlers concurrently.
    /// </summary>
    Parallel = 0,
    
    /// <summary>
    /// Executes notification handlers one at a time,
    /// waiting for each handler to complete before starting the next.
    /// </summary>
    Sequential = 1
}
