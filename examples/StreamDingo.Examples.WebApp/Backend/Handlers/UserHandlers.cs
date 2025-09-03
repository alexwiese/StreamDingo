using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles user creation events.
/// </summary>
public class UserCreatedHandler : IEventHandler<UserAggregate, UserCreated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, UserCreated @event)
    {
        var snapshot = previousSnapshot ?? new UserAggregate();
        
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
        
        snapshot.User = user;
        return snapshot;
    }
}

/// <summary>
/// Handles user update events.
/// </summary>
public class UserUpdatedHandler : IEventHandler<UserAggregate, UserUpdated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, UserUpdated @event)
    {
        var snapshot = previousSnapshot ?? new UserAggregate();
        
        if (snapshot.User != null)
        {
            snapshot.User.FirstName = @event.FirstName;
            snapshot.User.LastName = @event.LastName;
            snapshot.User.Email = @event.Email;
            snapshot.User.PhoneNumber = @event.PhoneNumber;
            snapshot.User.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}

/// <summary>
/// Handles user deletion events.
/// </summary>
public class UserDeletedHandler : IEventHandler<UserAggregate, UserDeleted>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, UserDeleted @event)
    {
        var snapshot = previousSnapshot ?? new UserAggregate();
        
        if (snapshot.User != null)
        {
            snapshot.User.IsDeleted = true;
            snapshot.User.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}