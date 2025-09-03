using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles user creation events.
/// </summary>
public class UserCreatedHandler : IEventHandler<DomainSnapshot, UserCreated>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, UserCreated @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
        var user = new User
        {
            Id = @event.UserId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            Email = @event.Email,
            PhoneNumber = @event.PhoneNumber,
            CreatedAt = @event.Timestamp,
            UpdatedAt = @event.Timestamp,
            IsDeleted = false
        };
        
        snapshot.Users[@event.UserId] = user;
        return snapshot;
    }
}

/// <summary>
/// Handles user update events.
/// </summary>
public class UserUpdatedHandler : IEventHandler<DomainSnapshot, UserUpdated>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, UserUpdated @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
        if (snapshot.Users.TryGetValue(@event.UserId, out var existingUser))
        {
            existingUser.FirstName = @event.FirstName;
            existingUser.LastName = @event.LastName;
            existingUser.Email = @event.Email;
            existingUser.PhoneNumber = @event.PhoneNumber;
            existingUser.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}

/// <summary>
/// Handles user deletion events.
/// </summary>
public class UserDeletedHandler : IEventHandler<DomainSnapshot, UserDeleted>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, UserDeleted @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
        if (snapshot.Users.TryGetValue(@event.UserId, out var existingUser))
        {
            existingUser.IsDeleted = true;
            existingUser.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}