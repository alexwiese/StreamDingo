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
    private readonly IEventStreamManager<DomainSnapshot> _streamManager;
    private const string StreamId = "domain-stream";

    public BusinessesController(IEventStreamManager<DomainSnapshot> streamManager)
    {
        _streamManager = streamManager;
    }

    /// <summary>
    /// Gets all businesses.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Business>>> GetBusinesses()
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        return Ok(snapshot?.ActiveBusinesses ?? Enumerable.Empty<Business>());
    }

    /// <summary>
    /// Gets a specific business by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Business>> GetBusiness(Guid id)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Businesses.TryGetValue(id, out var business) == true && !business.IsDeleted)
        {
            return Ok(business);
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
        var @event = new BusinessCreated
        {
            BusinessId = businessId,
            Name = request.Name,
            Description = request.Description,
            Industry = request.Industry,
            Address = request.Address,
            Website = request.Website,
            Version = await GetNextVersionAsync()
        };

        var snapshot = await _streamManager.AppendEventAsync(StreamId, @event, @event.Version - 1);
        var business = snapshot?.Businesses.GetValueOrDefault(businessId);
        
        return business != null ? CreatedAtAction(nameof(GetBusiness), new { id = businessId }, business) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing business.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Business>> UpdateBusiness(Guid id, UpdateBusinessRequest request)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Businesses.TryGetValue(id, out var existingBusiness) != true || existingBusiness.IsDeleted)
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
            Version = await GetNextVersionAsync()
        };

        var updatedSnapshot = await _streamManager.AppendEventAsync(StreamId, @event, @event.Version - 1);
        var updatedBusiness = updatedSnapshot?.Businesses.GetValueOrDefault(id);
        
        return updatedBusiness != null ? Ok(updatedBusiness) : StatusCode(500);
    }

    /// <summary>
    /// Deletes a business (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteBusiness(Guid id)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Businesses.TryGetValue(id, out var existingBusiness) != true || existingBusiness.IsDeleted)
        {
            return NotFound();
        }

        var @event = new BusinessDeleted
        {
            BusinessId = id,
            Version = await GetNextVersionAsync()
        };

        await _streamManager.AppendEventAsync(StreamId, @event, @event.Version - 1);
        return NoContent();
    }

    private async Task<long> GetNextVersionAsync()
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        return (snapshot?.Users.Count + snapshot?.Businesses.Count + snapshot?.Relationships.Count + 1) ?? 1;
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