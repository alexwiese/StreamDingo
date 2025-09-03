using Microsoft.AspNetCore.Mvc;
using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Controllers;

/// <summary>
/// API controller for managing businesses.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BusinessesController : ControllerBase
{
    private readonly IEventStreamManager<BusinessAggregate> _streamManager;

    public BusinessesController(IEventStreamManager<BusinessAggregate> streamManager)
    {
        _streamManager = streamManager;
    }

    /// <summary>
    /// Gets all businesses - this is a simplified implementation.
    /// In a real system, you'd typically have a read model or query all business streams.
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<Business>> GetBusinesses()
    {
        // Note: This is a simplified implementation for demo purposes.
        // In a production system, you'd typically maintain a read model 
        // or use a different approach to list all businesses.
        return Ok(new List<Business>());
    }

    /// <summary>
    /// Gets a specific business by ID using their individual stream.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Business>> GetBusiness(Guid id)
    {
        var streamId = $"business-{id}";
        var businessAggregate = await _streamManager.GetCurrentStateAsync(streamId);
        
        if (businessAggregate?.ActiveBusiness != null)
        {
            return Ok(businessAggregate.ActiveBusiness);
        }
        return NotFound();
    }

    /// <summary>
    /// Creates a new business.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Business>> CreateBusiness(CreateBusinessRequest request)
    {
        var businessId = Guid.NewGuid();
        var streamId = $"business-{businessId}";
        
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

        var currentVersion = await GetStreamVersionAsync(streamId);
        var businessAggregate = await _streamManager.AppendEventAsync(streamId, @event, currentVersion);
        var business = businessAggregate?.ActiveBusiness;
        
        return business != null ? CreatedAtAction(nameof(GetBusiness), new { id = businessId }, business) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing business.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Business>> UpdateBusiness(Guid id, UpdateBusinessRequest request)
    {
        var streamId = $"business-{id}";
        var businessAggregate = await _streamManager.GetCurrentStateAsync(streamId);
        
        if (businessAggregate?.ActiveBusiness == null)
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

        var currentVersion = await GetStreamVersionAsync(streamId);
        var updatedAggregate = await _streamManager.AppendEventAsync(streamId, @event, currentVersion);
        var updatedBusiness = updatedAggregate?.ActiveBusiness;
        
        return updatedBusiness != null ? Ok(updatedBusiness) : StatusCode(500);
    }

    /// <summary>
    /// Deletes a business (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteBusiness(Guid id)
    {
        var streamId = $"business-{id}";
        var businessAggregate = await _streamManager.GetCurrentStateAsync(streamId);
        
        if (businessAggregate?.ActiveBusiness == null)
        {
            return NotFound();
        }

        var @event = new BusinessDeleted
        {
            BusinessId = id,
            Version = 0
        };

        var currentVersion = await GetStreamVersionAsync(streamId);
        await _streamManager.AppendEventAsync(streamId, @event, currentVersion);
        return NoContent();
    }

    private async Task<long> GetStreamVersionAsync(string streamId)
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