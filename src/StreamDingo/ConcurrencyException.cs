namespace StreamDingo;

/// <summary>
/// Exception thrown when a concurrency conflict occurs during event store operations.
/// </summary>
public class ConcurrencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
    /// </summary>
    /// <param name="streamId">The identifier of the stream where the conflict occurred.</param>
    /// <param name="expectedVersion">The expected version.</param>
    /// <param name="actualVersion">The actual version.</param>
    public ConcurrencyException(string streamId, long expectedVersion, long actualVersion)
        : base($"Concurrency conflict in stream '{streamId}'. Expected version {expectedVersion}, but actual version is {actualVersion}.")
    {
        StreamId = streamId;
        ExpectedVersion = expectedVersion;
        ActualVersion = actualVersion;
    }

    /// <summary>
    /// Gets the identifier of the stream where the conflict occurred.
    /// </summary>
    public string StreamId { get; }

    /// <summary>
    /// Gets the expected version.
    /// </summary>
    public long ExpectedVersion { get; }

    /// <summary>
    /// Gets the actual version.
    /// </summary>
    public long ActualVersion { get; }
}