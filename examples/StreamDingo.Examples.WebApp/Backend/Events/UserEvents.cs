using StreamDingo;

namespace StreamDingo.Examples.WebApp.Events;

/// <summary>
/// Base class for user events.
/// </summary>
public abstract class UserEvent : IEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public long Version { get; set; }
    public Guid UserId { get; set; }
}

/// <summary>
/// Event fired when a user is created.
/// </summary>
public class UserCreated : UserEvent
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// Event fired when a user is updated.
/// </summary>
public class UserUpdated : UserEvent
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// Event fired when a user is deleted (soft delete).
/// </summary>
public class UserDeleted : UserEvent
{
}