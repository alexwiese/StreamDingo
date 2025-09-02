namespace StreamDingo;

/// <summary>
/// Base implementation of a snapshot with common properties
/// </summary>
/// <typeparam name="TEntity">The type of entity this snapshot represents</typeparam>
public class Snapshot<TEntity> : ISnapshot<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the Snapshot class
    /// </summary>
    /// <param name="entityId">The unique identifier of the entity</param>
    /// <param name="version">The version number of this snapshot</param>
    /// <param name="hash">The hash of this snapshot's data</param>
    /// <param name="data">The actual entity data</param>
    /// <param name="isKeySnapshot">Whether this is a key snapshot</param>
    public Snapshot(string entityId, long version, string hash, TEntity data, bool isKeySnapshot = false)
    {
        EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
        Version = version;
        Hash = hash ?? throw new ArgumentNullException(nameof(hash));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        IsKeySnapshot = isKeySnapshot;
        Timestamp = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc />
    public string EntityId { get; }

    /// <inheritdoc />
    public long Version { get; }

    /// <inheritdoc />
    public string Hash { get; }

    /// <inheritdoc />
    public DateTimeOffset Timestamp { get; }

    /// <inheritdoc />
    public TEntity Data { get; }

    /// <inheritdoc />
    public bool IsKeySnapshot { get; }
}