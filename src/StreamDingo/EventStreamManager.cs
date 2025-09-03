using System.Collections.Concurrent;

namespace StreamDingo;

/// <summary>
/// Default implementation of <see cref="IEventStreamManager{TSnapshot}"/>.
/// </summary>
/// <typeparam name="TSnapshot">The type of snapshot data.</typeparam>
public class EventStreamManager<TSnapshot> : IEventStreamManager<TSnapshot> where TSnapshot : class
{
    private readonly IEventStore _eventStore;
    private readonly ISnapshotStore<TSnapshot> _snapshotStore;
    private readonly IHashProvider _hashProvider;
    private readonly ConcurrentDictionary<Type, object> _handlers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStreamManager{TSnapshot}"/> class.
    /// </summary>
    /// <param name="eventStore">The event store.</param>
    /// <param name="snapshotStore">The snapshot store.</param>
    /// <param name="hashProvider">The hash provider.</param>
    public EventStreamManager(IEventStore eventStore, ISnapshotStore<TSnapshot> snapshotStore, IHashProvider hashProvider)
    {
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
    }

    /// <inheritdoc />
    public void RegisterHandler<TEvent>(IEventHandler<TSnapshot, TEvent> handler) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(handler);
        _handlers.TryAdd(typeof(TEvent), handler);
    }

    /// <inheritdoc />
    public async Task<TSnapshot?> ReplayAsync(string streamId, long fromVersion = 0, CancellationToken cancellationToken = default)
    {
        TSnapshot? currentState = default;

        await foreach (var @event in _eventStore.ReadEventsAsync(streamId, fromVersion, cancellationToken))
        {
            currentState = ApplyEvent(currentState, @event);
        }

        return currentState;
    }

    /// <inheritdoc />
    public async Task<TSnapshot?> GetCurrentStateAsync(string streamId, CancellationToken cancellationToken = default)
    {
        // Start with the latest snapshot if available
        var snapshot = await _snapshotStore.GetLatestSnapshotAsync(streamId, cancellationToken);
        long startVersion = snapshot?.Version + 1 ?? 0;
        var currentState = snapshot?.Data;

        // Apply any events after the snapshot
        await foreach (var @event in _eventStore.ReadEventsAsync(streamId, startVersion, cancellationToken))
        {
            currentState = ApplyEvent(currentState, @event);
        }

        return currentState;
    }

    /// <inheritdoc />
    public async Task<Snapshot<TSnapshot>?> CreateSnapshotAsync(string streamId, long atVersion, CancellationToken cancellationToken = default)
    {
        var state = await ReplayAsync(streamId, 0, cancellationToken);
        string hash = _hashProvider.CalculateHash(state!);
        var snapshot = new Snapshot<TSnapshot>(state, atVersion, DateTimeOffset.UtcNow, hash);

        await _snapshotStore.SaveSnapshotAsync(streamId, snapshot, cancellationToken);
        return snapshot;
    }

    /// <inheritdoc />
    public async Task<TSnapshot?> AppendEventAsync(string streamId, IEvent @event, long expectedVersion, CancellationToken cancellationToken = default)
    {
        // Append the event first
        await _eventStore.AppendEventAsync(streamId, @event, expectedVersion, cancellationToken);

        // Get the current state and apply the new event
        var currentState = await GetCurrentStateAsync(streamId, cancellationToken);
        return currentState;
    }

    /// <summary>
    /// Applies an event to the current state using the registered handler.
    /// </summary>
    /// <param name="currentState">The current state.</param>
    /// <param name="event">The event to apply.</param>
    /// <returns>The new state after applying the event.</returns>
    private TSnapshot? ApplyEvent(TSnapshot? currentState, IEvent @event)
    {
        var eventType = @event.GetType();

        if (!_handlers.TryGetValue(eventType, out object? handlerObj))
        {
            // If no handler is registered, return the state unchanged
            return currentState;
        }

        // Use reflection to call the handler's Handle method
        var handlerType = handlerObj.GetType();
        var handleMethod = handlerType.GetMethod("Handle", new[] { typeof(TSnapshot), eventType });

        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handler {handlerType.Name} does not have a Handle method that accepts {typeof(TSnapshot).Name} and {eventType.Name}");
        }

        try
        {
            object? result = handleMethod.Invoke(handlerObj, new object?[] { currentState, @event });
            return (TSnapshot?)result;
        }
        catch (Exception ex)
        {
            // Log the exception in a real implementation
            throw new InvalidOperationException($"Failed to apply event {eventType.Name} using handler {handlerType.Name}: {ex.Message}", ex);
        }
    }
}
