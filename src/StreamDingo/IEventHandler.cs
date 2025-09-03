namespace StreamDingo;

/// <summary>
/// Represents an event handler that transforms a snapshot by applying an event.
/// Event handlers are pure functions: (PreviousSnapshot, Event) -> NewSnapshot
/// </summary>
/// <typeparam name="TSnapshot">The type of snapshot/state being managed.</typeparam>
/// <typeparam name="TEvent">The type of event being handled.</typeparam>
public interface IEventHandler<TSnapshot, in TEvent> 
    where TEvent : IEvent
{
    /// <summary>
    /// Applies the event to the previous snapshot to produce a new snapshot.
    /// This method should be pure (no side effects) and deterministic.
    /// </summary>
    /// <param name="previousSnapshot">The previous snapshot state. May be null for the first event.</param>
    /// <param name="event">The event to apply.</param>
    /// <returns>The new snapshot after applying the event.</returns>
    TSnapshot Handle(TSnapshot? previousSnapshot, TEvent @event);

    /// <summary>
    /// Gets the type of event this handler can process.
    /// </summary>
    Type EventType => typeof(TEvent);
}