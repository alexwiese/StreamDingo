namespace StreamDingo;

/// <summary>
/// Represents a snapshot of an entity at a specific point in time
/// </summary>
/// <typeparam name="TEntity">The type of entity this snapshot represents</typeparam>
public interface ISnapshot<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets the unique identifier of the entity
    /// </summary>
    string EntityId { get; }

    /// <summary>
    /// Gets the version number of this snapshot
    /// </summary>
    long Version { get; }

    /// <summary>
    /// Gets the hash of this snapshot's data
    /// </summary>
    string Hash { get; }

    /// <summary>
    /// Gets the timestamp when this snapshot was created
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the actual entity data
    /// </summary>
    TEntity Data { get; }

    /// <summary>
    /// Gets whether this is a key snapshot (used as a checkpoint for replay)
    /// </summary>
    bool IsKeySnapshot { get; }
}