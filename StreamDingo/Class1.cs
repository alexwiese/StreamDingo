namespace StreamDingo;

/// <summary>
/// Base interface for events in the event sourcing system
/// </summary>
public interface IEvent
{
    string EventId { get; }
    DateTime Timestamp { get; }
}

/// <summary>
/// Base interface for snapshots with hash verification
/// </summary>
public interface ISnapshot
{
    string Hash { get; }
    DateTime CreatedAt { get; }
}

/// <summary>
/// Base interface for event handlers that apply events to snapshots
/// </summary>
/// <typeparam name="TSnapshot">The snapshot type</typeparam>
/// <typeparam name="TEvent">The event type</typeparam>
public interface IEventHandler<TSnapshot, TEvent>
    where TSnapshot : ISnapshot
    where TEvent : IEvent
{
    TSnapshot Apply(TSnapshot previousSnapshot, TEvent @event);
    string HandlerHash { get; }
}
