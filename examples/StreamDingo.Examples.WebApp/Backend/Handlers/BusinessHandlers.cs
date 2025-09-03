using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles business creation events.
/// </summary>
public class BusinessCreatedHandler : IEventHandler<BusinessAggregate, BusinessCreated>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, BusinessCreated @event)
    {
        var snapshot = previousSnapshot ?? new BusinessAggregate();
        
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
        
        snapshot.Business = business;
        return snapshot;
    }
}

/// <summary>
/// Handles business update events.
/// </summary>
public class BusinessUpdatedHandler : IEventHandler<BusinessAggregate, BusinessUpdated>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, BusinessUpdated @event)
    {
        var snapshot = previousSnapshot ?? new BusinessAggregate();
        
        if (snapshot.Business != null)
        {
            snapshot.Business.Name = @event.Name;
            snapshot.Business.Description = @event.Description;
            snapshot.Business.Industry = @event.Industry;
            snapshot.Business.Address = @event.Address;
            snapshot.Business.Website = @event.Website;
            snapshot.Business.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}

/// <summary>
/// Handles business deletion events.
/// </summary>
public class BusinessDeletedHandler : IEventHandler<BusinessAggregate, BusinessDeleted>
{
    public BusinessAggregate Handle(BusinessAggregate? previousSnapshot, BusinessDeleted @event)
    {
        var snapshot = previousSnapshot ?? new BusinessAggregate();
        
        if (snapshot.Business != null)
        {
            snapshot.Business.IsDeleted = true;
            snapshot.Business.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}