using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles relationship creation events.
/// </summary>
public class RelationshipCreatedHandler : IEventHandler<DomainSnapshot, RelationshipCreated>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, RelationshipCreated @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
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
public class RelationshipUpdatedHandler : IEventHandler<DomainSnapshot, RelationshipUpdated>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, RelationshipUpdated @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
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
public class RelationshipDeletedHandler : IEventHandler<DomainSnapshot, RelationshipDeleted>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, RelationshipDeleted @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
        if (snapshot.Relationships.TryGetValue(@event.RelationshipId, out var existingRelationship))
        {
            existingRelationship.IsDeleted = true;
            existingRelationship.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}