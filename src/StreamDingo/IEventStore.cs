namespace StreamDingo;

/// <summary>
/// Provides storage and retrieval functionality for events.
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Appends an event to the specified stream.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="event">The event to append.</param>
    /// <param name="expectedVersion">The expected current version of the stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ConcurrencyException">Thrown when the expected version doesn't match the actual version.</exception>
    Task AppendEventAsync(string streamId, IEvent @event, long expectedVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends multiple events to the specified stream atomically.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="events">The events to append.</param>
    /// <param name="expectedVersion">The expected current version of the stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ConcurrencyException">Thrown when the expected version doesn't match the actual version.</exception>
    Task AppendEventsAsync(string streamId, IEnumerable<IEvent> events, long expectedVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all events from the specified stream starting from the given version.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="fromVersion">The version to start reading from (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of events.</returns>
    IAsyncEnumerable<IEvent> ReadEventsAsync(string streamId, long fromVersion = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current version of the specified stream.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current version, or -1 if the stream doesn't exist.</returns>
    Task<long> GetStreamVersionAsync(string streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the specified stream exists.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the stream exists; otherwise, false.</returns>
    Task<bool> StreamExistsAsync(string streamId, CancellationToken cancellationToken = default);
}