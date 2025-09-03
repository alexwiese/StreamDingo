using Microsoft.AspNetCore.Mvc;
using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Controllers;

/// <summary>
/// API controller for managing businesses with aggregate-based event sourcing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BusinessesController : ControllerBase
{
    private readonly IEventStreamManager<BusinessAggregate> _businessStreamManager;
    private readonly IEventStreamManager<DomainSnapshot> _domainStreamManager;

    public BusinessesController(
        IEventStreamManager<BusinessAggregate> businessStreamManager,
        IEventStreamManager<DomainSnapshot> domainStreamManager)
    {
        _businessStreamManager = businessStreamManager;
        _domainStreamManager = domainStreamManager;
    }

    /// <summary>
    /// Gets all businesses (using legacy domain stream for now).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Business>>> GetBusinesses()
    {
        // For listing all businesses, we still use the domain stream
        // In a real system, this might be a projection/read model
        const string domainStreamId = "domain-stream";
        var snapshot = await _domainStreamManager.GetCurrentStateAsync(domainStreamId);
        return Ok(snapshot?.ActiveBusinesses ?? Enumerable.Empty<Business>());
    }

    /// <summary>
    /// Gets a specific business by ID using aggregate-based querying.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Business>> GetBusiness(Guid id)
    {
        var streamId = $"business-{id}";
        var aggregate = await _businessStreamManager.GetCurrentStateAsync(streamId);
        
        if (aggregate?.HasBusiness == true)
        {
            return Ok(aggregate.Business);
        }
        return NotFound();
    }

    /// <summary>
    /// Gets relationships for a specific business using aggregate-based querying.
    /// </summary>
    [HttpGet("{id:guid}/relationships")]
    public async Task<ActionResult<IEnumerable<Relationship>>> GetBusinessRelationships(Guid id)
    {
        var streamId = $"business-{id}";
        var aggregate = await _businessStreamManager.GetCurrentStateAsync(streamId);
        
        if (aggregate?.HasBusiness == true)
        {
            return Ok(aggregate.ActiveRelationships);
        }
        return NotFound();
    }

    /// <summary>
    /// Creates a new business using aggregate-based event sourcing.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Business>> CreateBusiness(CreateBusinessRequest request)
    {
        var businessId = Guid.NewGuid();
        var businessStreamId = $"business-{businessId}";
        var domainStreamId = "domain-stream";
        
        var @event = new BusinessCreated
        {
            BusinessId = businessId,
            Name = request.Name,
            Description = request.Description,
            Industry = request.Industry,
            Address = request.Address,
            Website = request.Website,
            Version = 0
        };

        // Append to both aggregate stream and domain stream
        var businessCurrentVersion = await GetCurrentVersionAsync(businessStreamId);
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        
        var businessAggregate = await _businessStreamManager.AppendEventAsync(businessStreamId, @event, businessCurrentVersion);
        await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        
        var business = businessAggregate?.Business;
        return business != null ? CreatedAtAction(nameof(GetBusiness), new { id = businessId }, business) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing business using aggregate-based event sourcing.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Business>> UpdateBusiness(Guid id, UpdateBusinessRequest request)
    {
        var businessStreamId = $"business-{id}";
        var domainStreamId = "domain-stream";
        
        // Check if business exists in aggregate
        var aggregate = await _businessStreamManager.GetCurrentStateAsync(businessStreamId);
        if (aggregate?.HasBusiness != true)
        {
            return NotFound();
        }

        var @event = new BusinessUpdated
        {
            BusinessId = id,
            Name = request.Name,
            Description = request.Description,
            Industry = request.Industry,
            Address = request.Address,
            Website = request.Website,
            Version = 0
        };

        // Append to both aggregate stream and domain stream
        var businessCurrentVersion = await GetCurrentVersionAsync(businessStreamId);
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        
        var updatedAggregate = await _businessStreamManager.AppendEventAsync(businessStreamId, @event, businessCurrentVersion);
        await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        
        var updatedBusiness = updatedAggregate?.Business;
        return updatedBusiness != null ? Ok(updatedBusiness) : StatusCode(500);
    }

    /// <summary>
    /// Deletes a business (soft delete) using aggregate-based event sourcing.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteBusiness(Guid id)
    {
        var businessStreamId = $"business-{id}";
        var domainStreamId = "domain-stream";
        
        // Check if business exists in aggregate
        var aggregate = await _businessStreamManager.GetCurrentStateAsync(businessStreamId);
        if (aggregate?.HasBusiness != true)
        {
            return NotFound();
        }

        var @event = new BusinessDeleted
        {
            BusinessId = id,
            Version = 0
        };

        // Append to both aggregate stream and domain stream
        var businessCurrentVersion = await GetCurrentVersionAsync(businessStreamId);
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        
        await _businessStreamManager.AppendEventAsync(businessStreamId, @event, businessCurrentVersion);
        await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        
        return NoContent();
    }

    private async Task<long> GetCurrentVersionAsync(string streamId)
    {
        var eventStore = HttpContext.RequestServices.GetRequiredService<IEventStore>();
        return await eventStore.GetStreamVersionAsync(streamId);
    }
}

/// <summary>
/// Request model for creating a business.
/// </summary>
public class CreateBusinessRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating a business.
/// </summary>
public class UpdateBusinessRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}