namespace StreamDingo;

/// <summary>
/// Handles the application of events to entity snapshots
/// </summary>
/// <typeparam name="TEntity">The type of entity this handler processes</typeparam>
/// <typeparam name="TEvent">The type of event this handler processes</typeparam>
public interface IEventHandler<TEntity, TEvent>
    where TEntity : class
    where TEvent : class, IEvent<TEntity>
{
    /// <summary>
    /// Gets the hash of this event handler's code
    /// </summary>
    string CodeHash { get; }

    /// <summary>
    /// Applies the event to the previous snapshot to create a new snapshot
    /// </summary>
    /// <param name="previousSnapshot">The previous snapshot of the entity</param>
    /// <param name="event">The event to apply</param>
    /// <returns>A new snapshot with the event applied</returns>
    ISnapshot<TEntity> Apply(ISnapshot<TEntity> previousSnapshot, TEvent @event);
}