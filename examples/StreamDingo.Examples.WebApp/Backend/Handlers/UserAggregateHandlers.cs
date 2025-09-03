using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles user creation events for UserAggregate.
/// </summary>
public class UserAggregateCreatedHandler : IEventHandler<UserAggregate, UserCreated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, UserCreated @event)
    {
        var aggregate = previousSnapshot ?? new UserAggregate();
        
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
        
        aggregate.User = user;
        return aggregate;
    }
}

/// <summary>
/// Handles user update events for UserAggregate.
/// </summary>
public class UserAggregateUpdatedHandler : IEventHandler<UserAggregate, UserUpdated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, UserUpdated @event)
    {
        var aggregate = previousSnapshot ?? new UserAggregate();
        
        if (aggregate.User != null)
        {
            aggregate.User.FirstName = @event.FirstName;
            aggregate.User.LastName = @event.LastName;
            aggregate.User.Email = @event.Email;
            aggregate.User.PhoneNumber = @event.PhoneNumber;
            aggregate.User.UpdatedAt = @event.Timestamp;
        }
        
        return aggregate;
    }
}

/// <summary>
/// Handles user deletion events for UserAggregate.
/// </summary>
public class UserAggregateDeletedHandler : IEventHandler<UserAggregate, UserDeleted>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, UserDeleted @event)
    {
        var aggregate = previousSnapshot ?? new UserAggregate();
        
        if (aggregate.User != null)
        {
            aggregate.User.IsDeleted = true;
            aggregate.User.UpdatedAt = @event.Timestamp;
        }
        
        return aggregate;
    }
}

/// <summary>
/// Handles relationship creation events for UserAggregate (when user is involved).
/// </summary>
public class UserAggregateRelationshipCreatedHandler : IEventHandler<UserAggregate, RelationshipCreated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, RelationshipCreated @event)
    {
        var aggregate = previousSnapshot ?? new UserAggregate();
        
        // Only add relationship if this user is involved
        if (aggregate.User != null && @event.UserId == aggregate.User.Id)
        {
            var relationship = new Relationship
            {
                Id = @event.RelationshipId,
                UserId = @event.UserId,
                BusinessId = @event.BusinessId,
                Type = @event.Type,
                Title = @event.Title,
                Description = @event.Description,
                StartDate = @event.StartDate,
                CreatedAt = @event.Timestamp,
                UpdatedAt = @event.Timestamp,
                IsActive = true,
                IsDeleted = false
            };
            
            aggregate.Relationships[@event.RelationshipId] = relationship;
        }
        
        return aggregate;
    }
}

/// <summary>
/// Handles relationship update events for UserAggregate.
/// </summary>
public class UserAggregateRelationshipUpdatedHandler : IEventHandler<UserAggregate, RelationshipUpdated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, RelationshipUpdated @event)
    {
        var aggregate = previousSnapshot ?? new UserAggregate();
        
        if (aggregate.Relationships.TryGetValue(@event.RelationshipId, out var existingRelationship))
        {
            existingRelationship.Type = @event.Type;
            existingRelationship.Title = @event.Title;
            existingRelationship.Description = @event.Description;
            existingRelationship.StartDate = @event.StartDate;
            existingRelationship.EndDate = @event.EndDate;
            existingRelationship.IsActive = @event.IsActive;
            existingRelationship.UpdatedAt = @event.Timestamp;
        }
        
        return aggregate;
    }
}

/// <summary>
/// Handles relationship deletion events for UserAggregate.
/// </summary>
public class UserAggregateRelationshipDeletedHandler : IEventHandler<UserAggregate, RelationshipDeleted>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, RelationshipDeleted @event)
    {
        var aggregate = previousSnapshot ?? new UserAggregate();
        
        if (aggregate.Relationships.TryGetValue(@event.RelationshipId, out var existingRelationship))
        {
            existingRelationship.IsDeleted = true;
            existingRelationship.UpdatedAt = @event.Timestamp;
        }
        
        return aggregate;
    }
}