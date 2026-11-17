namespace EvoTechBuilder.Mediator.Abstractions.Contracts;

/// <summary>
/// Defines a handler responsible for processing a notification.
/// </summary>
/// <typeparam name="TNotification">
/// The type of notification being handled.
/// </typeparam>
public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    /// <summary>
    /// Handles the specified notification asynchronously.
    /// </summary>
    /// <param name="notification">
    /// The notification to process.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    ValueTask Handle(TNotification notification, CancellationToken cancellationToken);
}
