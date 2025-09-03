namespace StreamDingo;

/// <summary>
/// Represents a snapshot of entity state at a particular point in time.
/// </summary>
/// <typeparam name="TSnapshot">The type of the snapshot data.</typeparam>
public class Snapshot<TSnapshot> where TSnapshot : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Snapshot{TSnapshot}"/> class.
    /// </summary>
    /// <param name="data">The snapshot data.</param>
    /// <param name="version">The version at which this snapshot was taken.</param>
    /// <param name="timestamp">The timestamp when this snapshot was created.</param>
    /// <param name="hash">The hash of the snapshot data for integrity verification.</param>
    public Snapshot(TSnapshot? data, long version, DateTimeOffset timestamp, string hash)
    {
        Data = data;
        Version = version;
        Timestamp = timestamp;
        Hash = hash;
    }

    /// <summary>
    /// Gets the snapshot data.
    /// </summary>
    public TSnapshot? Data { get; }

    /// <summary>
    /// Gets the version at which this snapshot was taken.
    /// </summary>
    public long Version { get; }

    /// <summary>
    /// Gets the timestamp when this snapshot was created.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the hash of the snapshot data for integrity verification.
    /// </summary>
    public string Hash { get; }
}
