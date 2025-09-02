namespace StreamDingo;

/// <summary>
/// Base implementation of an event with common properties
/// </summary>
/// <typeparam name="TEntity">The type of entity this event applies to</typeparam>
public abstract class EventBase<TEntity> : IEvent<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the EventBase class
    /// </summary>
    /// <param name="handlerCodeHash">The hash of the event handler code</param>
    protected EventBase(string handlerCodeHash)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTimeOffset.UtcNow;
        HandlerCodeHash = handlerCodeHash ?? throw new ArgumentNullException(nameof(handlerCodeHash));
    }

    /// <inheritdoc />
    public Guid EventId { get; }

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; }

    /// <inheritdoc />
    public string HandlerCodeHash { get; }
}