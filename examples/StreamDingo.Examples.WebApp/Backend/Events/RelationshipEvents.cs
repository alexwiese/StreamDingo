using StreamDingo;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Events;

/// <summary>
/// Base class for relationship events.
/// </summary>
public abstract class RelationshipEvent : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public long Version { get; set; }
    public Guid RelationshipId { get; set; }
}

/// <summary>
/// Event fired when a relationship is created.
/// </summary>
public class RelationshipCreated : RelationshipEvent
{
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    public RelationshipType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
}

/// <summary>
/// Event fired when a relationship is updated.
/// </summary>
public class RelationshipUpdated : RelationshipEvent
{
    public RelationshipType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Event fired when a relationship is deleted (soft delete).
/// </summary>
public class RelationshipDeleted : RelationshipEvent
{
}