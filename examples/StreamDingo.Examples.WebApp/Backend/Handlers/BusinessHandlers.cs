using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Handlers;

/// <summary>
/// Handles business creation events.
/// </summary>
public class BusinessCreatedHandler : IEventHandler<DomainSnapshot, BusinessCreated>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, BusinessCreated @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
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
        
        snapshot.Businesses[@event.BusinessId] = business;
        return snapshot;
    }
}

/// <summary>
/// Handles business update events.
/// </summary>
public class BusinessUpdatedHandler : IEventHandler<DomainSnapshot, BusinessUpdated>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, BusinessUpdated @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
        if (snapshot.Businesses.TryGetValue(@event.BusinessId, out var existingBusiness))
        {
            existingBusiness.Name = @event.Name;
            existingBusiness.Description = @event.Description;
            existingBusiness.Industry = @event.Industry;
            existingBusiness.Address = @event.Address;
            existingBusiness.Website = @event.Website;
            existingBusiness.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}

/// <summary>
/// Handles business deletion events.
/// </summary>
public class BusinessDeletedHandler : IEventHandler<DomainSnapshot, BusinessDeleted>
{
    public DomainSnapshot Handle(DomainSnapshot? previousSnapshot, BusinessDeleted @event)
    {
        var snapshot = previousSnapshot ?? new DomainSnapshot();
        
        if (snapshot.Businesses.TryGetValue(@event.BusinessId, out var existingBusiness))
        {
            existingBusiness.IsDeleted = true;
            existingBusiness.UpdatedAt = @event.Timestamp;
        }
        
        return snapshot;
    }
}