using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles relationship creation events.
/// </summary>
public class RelationshipCreatedHandler : IEventHandler<UserAggregate, RelationshipCreated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, RelationshipCreated @event)
    {
        var snapshot = previousSnapshot ?? new UserAggregate();
        
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
        
        snapshot.Relationships[@event.RelationshipId] = relationship;
        return snapshot;
    }
}

/// <summary>
/// Handles relationship update events.
/// </summary>
public class RelationshipUpdatedHandler : IEventHandler<UserAggregate, RelationshipUpdated>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, RelationshipUpdated @event)
    {
        var snapshot = previousSnapshot ?? new UserAggregate();
        
        if (snapshot.Relationships.TryGetValue(@event.RelationshipId, out var existingRelationship))
        {
            existingRelationship.Type = @event.Type;
            existingRelationship.Title = @event.Title;
            existingRelationship.Description = @event.Description;
            existingRelationship.StartDate = @event.StartDate;
            existingRelationship.EndDate = @event.EndDate;
            existingRelationship.IsActive = @event.IsActive;
            existingRelationship.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}

/// <summary>
/// Handles relationship deletion events.
/// </summary>
public class RelationshipDeletedHandler : IEventHandler<UserAggregate, RelationshipDeleted>
{
    public UserAggregate Handle(UserAggregate? previousSnapshot, RelationshipDeleted @event)
    {
        var snapshot = previousSnapshot ?? new UserAggregate();
        
        if (snapshot.Relationships.TryGetValue(@event.RelationshipId, out var existingRelationship))
        {
            existingRelationship.IsDeleted = true;
            existingRelationship.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}