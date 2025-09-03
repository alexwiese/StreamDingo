namespace StreamDingo;

/// <summary>
/// Manages event streams, including event replay and snapshot generation.
/// </summary>
/// <typeparam name="TSnapshot">The type of snapshot data.</typeparam>
public interface IEventStreamManager<TSnapshot> where TSnapshot : class
{
    /// <summary>
    /// Replays events from the stream to rebuild the current state.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="fromVersion">The version to start replay from (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current state after replaying events.</returns>
    Task<TSnapshot?> ReplayAsync(string streamId, long fromVersion = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current state of the stream by using the latest snapshot and replaying subsequent events.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current state of the stream.</returns>
    Task<TSnapshot?> GetCurrentStateAsync(string streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a snapshot of the current state at the specified version.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="atVersion">The version at which to create the snapshot.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created snapshot.</returns>
    Task<Snapshot<TSnapshot>?> CreateSnapshotAsync(string streamId, long atVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers an event handler for the specified event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to handle.</typeparam>
    /// <param name="handler">The event handler.</param>
    void RegisterHandler<TEvent>(IEventHandler<TSnapshot, TEvent> handler) where TEvent : IEvent;

    /// <summary>
    /// Appends an event to the stream and updates the current state.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="expectedVersion">The expected current version of the stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The new state after applying the event.</returns>
    Task<TSnapshot?> AppendEventAsync(string streamId, IEvent @event, long expectedVersion, CancellationToken cancellationToken = default);
}