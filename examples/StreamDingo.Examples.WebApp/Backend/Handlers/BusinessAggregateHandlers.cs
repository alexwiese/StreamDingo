using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles business creation events for BusinessAggregate.
/// </summary>
public class BusinessAggregateCreatedHandler : IEventHandler<BusinessAggregate, BusinessCreated>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, BusinessCreated @event)
    {
        var aggregate = previousSnapshot ?? new BusinessAggregate();
        
        var business = new Business
        {
            Id = @event.BusinessId,
            Name = @event.Name,
            Description = @event.Description,
            Industry = @event.Industry,
            Address = @event.Address,
            Website = @event.Website,
            CreatedAt = @event.Timestamp,
            UpdatedAt = @event.Timestamp,
            IsDeleted = false
        };
        
        aggregate.Business = business;
        return aggregate;
    }
}

/// <summary>
/// Handles business update events for BusinessAggregate.
/// </summary>
public class BusinessAggregateUpdatedHandler : IEventHandler<BusinessAggregate, BusinessUpdated>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, BusinessUpdated @event)
    {
        var aggregate = previousSnapshot ?? new BusinessAggregate();
        
        if (aggregate.Business != null)
        {
            aggregate.Business.Name = @event.Name;
            aggregate.Business.Description = @event.Description;
            aggregate.Business.Industry = @event.Industry;
            aggregate.Business.Address = @event.Address;
            aggregate.Business.Website = @event.Website;
            aggregate.Business.UpdatedAt = @event.Timestamp;
        }
        
        return aggregate;
    }
}

/// <summary>
/// Handles business deletion events for BusinessAggregate.
/// </summary>
public class BusinessAggregateDeletedHandler : IEventHandler<BusinessAggregate, BusinessDeleted>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, BusinessDeleted @event)
    {
        var aggregate = previousSnapshot ?? new BusinessAggregate();
        
        if (aggregate.Business != null)
        {
            aggregate.Business.IsDeleted = true;
            aggregate.Business.UpdatedAt = @event.Timestamp;
        }
        
        return aggregate;
    }
}

/// <summary>
/// Handles relationship creation events for BusinessAggregate (when business is involved).
/// </summary>
public class BusinessAggregateRelationshipCreatedHandler : IEventHandler<BusinessAggregate, RelationshipCreated>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, RelationshipCreated @event)
    {
        var aggregate = previousSnapshot ?? new BusinessAggregate();
        
        // Only add relationship if this business is involved
        if (aggregate.Business != null && @event.BusinessId == aggregate.Business.Id)
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
/// Handles relationship update events for BusinessAggregate.
/// </summary>
public class BusinessAggregateRelationshipUpdatedHandler : IEventHandler<BusinessAggregate, RelationshipUpdated>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, RelationshipUpdated @event)
    {
        var aggregate = previousSnapshot ?? new BusinessAggregate();
        
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
/// Handles relationship deletion events for BusinessAggregate.
/// </summary>
public class BusinessAggregateRelationshipDeletedHandler : IEventHandler<BusinessAggregate, RelationshipDeleted>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, RelationshipDeleted @event)
    {
        var aggregate = previousSnapshot ?? new BusinessAggregate();
        
        if (aggregate.Relationships.TryGetValue(@event.RelationshipId, out var existingRelationship))
        {
            existingRelationship.IsDeleted = true;
            existingRelationship.UpdatedAt = @event.Timestamp;
        }
        
        return aggregate;
    }
}