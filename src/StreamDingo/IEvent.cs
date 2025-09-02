namespace StreamDingo;

/// <summary>
/// Represents an event that can be applied to an entity to change its state
/// </summary>
/// <typeparam name="TEntity">The type of entity this event applies to</typeparam>
public interface IEvent<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets the unique identifier for this event
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the timestamp when this event occurred
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the hash of the event handler code that processes this event
    /// </summary>
    string HandlerCodeHash { get; }
}