using StreamDingo;

namespace StreamDingo.Examples.WebApp.Events;

/// <summary>
/// Base class for business events.
/// </summary>
public abstract class BusinessEvent : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public long Version { get; set; }
    public Guid BusinessId { get; set; }
}

/// <summary>
/// Event fired when a business is created.
/// </summary>
public class BusinessCreated : BusinessEvent
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}

/// <summary>
/// Event fired when a business is updated.
/// </summary>
public class BusinessUpdated : BusinessEvent
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}

/// <summary>
/// Event fired when a business is deleted (soft delete).
/// </summary>
public class BusinessDeleted : BusinessEvent
{
}