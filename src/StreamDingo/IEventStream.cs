namespace StreamDingo;

/// <summary>
/// Provides functionality for managing event streams and entity snapshots
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by this stream</typeparam>
public interface IEventStream<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets the unique identifier of the entity stream
    /// </summary>
    string EntityId { get; }

    /// <summary>
    /// Appends an event to the stream
    /// </summary>
    /// <param name="event">The event to append</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>The new snapshot after applying the event</returns>
    Task<ISnapshot<TEntity>> AppendEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent<TEntity>;

    /// <summary>
    /// Gets the current snapshot of the entity
    /// </summary>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>The current snapshot, or null if no events have been applied</returns>
    Task<ISnapshot<TEntity>?> GetCurrentSnapshotAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all events in the stream
    /// </summary>
    /// <param name="fromVersion">The version to start from (inclusive)</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>An enumerable of events ordered by version</returns>
    IAsyncEnumerable<IEvent<TEntity>> GetEventsAsync(long fromVersion = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replays events from a specific snapshot to rebuild the current state
    /// This is used when event handler code changes or event order changes
    /// </summary>
    /// <param name="fromSnapshot">The snapshot to start replay from</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests</param>
    /// <returns>The final snapshot after replay</returns>
    Task<ISnapshot<TEntity>> ReplayFromSnapshotAsync(ISnapshot<TEntity> fromSnapshot, CancellationToken cancellationToken = default);
}