namespace StreamDingo;

/// <summary>
/// Represents an event in the event sourcing system.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the unique identifier for this event.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the version number of this event in the stream.
    /// </summary>
    long Version { get; }
}