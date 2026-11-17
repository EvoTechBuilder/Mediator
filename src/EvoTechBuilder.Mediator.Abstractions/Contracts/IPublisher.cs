namespace EvoTechBuilder.Mediator.Abstractions.Contracts;

/// <summary>
/// Defines a component responsible for publishing notifications
/// to their corresponding handlers.
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    /// <typeparam name="TNotification">
    /// The type of notification being published.
    /// </typeparam>
    /// <param name="notification">
    /// The notification to publish.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous publishing operation.
    /// </returns>
    ValueTask Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
