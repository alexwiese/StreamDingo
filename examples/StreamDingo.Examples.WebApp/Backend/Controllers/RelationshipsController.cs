using Microsoft.AspNetCore.Mvc;
using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Controllers;

/// <summary>
/// API controller for managing relationships.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RelationshipsController : ControllerBase
{
    private readonly IEventStreamManager<DomainSnapshot> _streamManager;
    private const string StreamId = "domain-stream";

    public RelationshipsController(IEventStreamManager<DomainSnapshot> streamManager)
    {
        _streamManager = streamManager;
    }

    /// <summary>
    /// Gets all relationships.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetRelationships()
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        var relationships = snapshot?.ActiveRelationships ?? Enumerable.Empty<Relationship>();
        
        var relationshipDtos = relationships.Select(r => new RelationshipDto
        {
            Id = r.Id,
            UserId = r.UserId,
            BusinessId = r.BusinessId,
            Type = r.Type,
            Title = r.Title,
            Description = r.Description,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            UserName = snapshot?.Users.GetValueOrDefault(r.UserId)?.FullName ?? "Unknown",
            BusinessName = snapshot?.Businesses.GetValueOrDefault(r.BusinessId)?.Name ?? "Unknown"
        });

        return Ok(relationshipDtos);
    }

    /// <summary>
    /// Gets a specific relationship by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RelationshipDto>> GetRelationship(Guid id)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Relationships.TryGetValue(id, out var relationship) == true && !relationship.IsDeleted)
        {
            var relationshipDto = new RelationshipDto
            {
                Id = relationship.Id,
                UserId = relationship.UserId,
                BusinessId = relationship.BusinessId,
                Type = relationship.Type,
                Title = relationship.Title,
                Description = relationship.Description,
                StartDate = relationship.StartDate,
                EndDate = relationship.EndDate,
                IsActive = relationship.IsActive,
                CreatedAt = relationship.CreatedAt,
                UpdatedAt = relationship.UpdatedAt,
                UserName = snapshot.Users.GetValueOrDefault(relationship.UserId)?.FullName ?? "Unknown",
                BusinessName = snapshot.Businesses.GetValueOrDefault(relationship.BusinessId)?.Name ?? "Unknown"
            };
            
            return Ok(relationshipDto);
        }
        return NotFound();
    }

    /// <summary>
    /// Gets relationships for a specific user.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetUserRelationships(Guid userId)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        var relationships = snapshot?.GetUserRelationships(userId) ?? Enumerable.Empty<Relationship>();
        
        var relationshipDtos = relationships.Select(r => new RelationshipDto
        {
            Id = r.Id,
            UserId = r.UserId,
            BusinessId = r.BusinessId,
            Type = r.Type,
            Title = r.Title,
            Description = r.Description,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            UserName = snapshot?.Users.GetValueOrDefault(r.UserId)?.FullName ?? "Unknown",
            BusinessName = snapshot?.Businesses.GetValueOrDefault(r.BusinessId)?.Name ?? "Unknown"
        });

        return Ok(relationshipDtos);
    }

    /// <summary>
    /// Gets relationships for a specific business.
    /// </summary>
    [HttpGet("business/{businessId:guid}")]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetBusinessRelationships(Guid businessId)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        var relationships = snapshot?.GetBusinessRelationships(businessId) ?? Enumerable.Empty<Relationship>();
        
        var relationshipDtos = relationships.Select(r => new RelationshipDto
        {
            Id = r.Id,
            UserId = r.UserId,
            BusinessId = r.BusinessId,
            Type = r.Type,
            Title = r.Title,
            Description = r.Description,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            IsActive = r.IsActive,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            UserName = snapshot?.Users.GetValueOrDefault(r.UserId)?.FullName ?? "Unknown",
            BusinessName = snapshot?.Businesses.GetValueOrDefault(r.BusinessId)?.Name ?? "Unknown"
        });

        return Ok(relationshipDtos);
    }

    /// <summary>
    /// Creates a new relationship.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RelationshipDto>> CreateRelationship(CreateRelationshipRequest request)
    {
        // Validate that user and business exist
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Users.GetValueOrDefault(request.UserId)?.IsDeleted != false ||
            snapshot?.Businesses.GetValueOrDefault(request.BusinessId)?.IsDeleted != false)
        {
            return BadRequest("User or business not found or has been deleted.");
        }

        var relationshipId = Guid.NewGuid();
        var @event = new RelationshipCreated
        {
            RelationshipId = relationshipId,
            UserId = request.UserId,
            BusinessId = request.BusinessId,
            Type = request.Type,
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            Version = 0
        };

        var currentVersion = await GetCurrentVersionAsync();
        var updatedSnapshot = await _streamManager.AppendEventAsync(StreamId, @event, currentVersion);
        var relationship = updatedSnapshot?.Relationships.GetValueOrDefault(relationshipId);
        
        if (relationship != null)
        {
            var relationshipDto = new RelationshipDto
            {
                Id = relationship.Id,
                UserId = relationship.UserId,
                BusinessId = relationship.BusinessId,
                Type = relationship.Type,
                Title = relationship.Title,
                Description = relationship.Description,
                StartDate = relationship.StartDate,
                EndDate = relationship.EndDate,
                IsActive = relationship.IsActive,
                CreatedAt = relationship.CreatedAt,
                UpdatedAt = relationship.UpdatedAt,
                UserName = updatedSnapshot.Users.GetValueOrDefault(relationship.UserId)?.FullName ?? "Unknown",
                BusinessName = updatedSnapshot.Businesses.GetValueOrDefault(relationship.BusinessId)?.Name ?? "Unknown"
            };
            
            return CreatedAtAction(nameof(GetRelationship), new { id = relationshipId }, relationshipDto);
        }
        
        return StatusCode(500);
    }

    /// <summary>
    /// Updates an existing relationship.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RelationshipDto>> UpdateRelationship(Guid id, UpdateRelationshipRequest request)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Relationships.TryGetValue(id, out var existingRelationship) != true || existingRelationship.IsDeleted)
        {
            return NotFound();
        }

        var @event = new RelationshipUpdated
        {
            RelationshipId = id,
            Type = request.Type,
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            Version = 0
        };

        var currentVersion = await GetCurrentVersionAsync();
        var updatedSnapshot = await _streamManager.AppendEventAsync(StreamId, @event, currentVersion);
        var updatedRelationship = updatedSnapshot?.Relationships.GetValueOrDefault(id);
        
        if (updatedRelationship != null)
        {
            var relationshipDto = new RelationshipDto
            {
                Id = updatedRelationship.Id,
                UserId = updatedRelationship.UserId,
                BusinessId = updatedRelationship.BusinessId,
                Type = updatedRelationship.Type,
                Title = updatedRelationship.Title,
                Description = updatedRelationship.Description,
                StartDate = updatedRelationship.StartDate,
                EndDate = updatedRelationship.EndDate,
                IsActive = updatedRelationship.IsActive,
                CreatedAt = updatedRelationship.CreatedAt,
                UpdatedAt = updatedRelationship.UpdatedAt,
                UserName = updatedSnapshot.Users.GetValueOrDefault(updatedRelationship.UserId)?.FullName ?? "Unknown",
                BusinessName = updatedSnapshot.Businesses.GetValueOrDefault(updatedRelationship.BusinessId)?.Name ?? "Unknown"
            };
            
            return Ok(relationshipDto);
        }
        
        return StatusCode(500);
    }

    /// <summary>
    /// Deletes a relationship (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteRelationship(Guid id)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Relationships.TryGetValue(id, out var existingRelationship) != true || existingRelationship.IsDeleted)
        {
            return NotFound();
        }

        var @event = new RelationshipDeleted
        {
            RelationshipId = id,
            Version = 0
        };

        var currentVersion = await GetCurrentVersionAsync();
        await _streamManager.AppendEventAsync(StreamId, @event, currentVersion);
        return NoContent();
    }

    private async Task<long> GetCurrentVersionAsync()
    {
        var eventStore = HttpContext.RequestServices.GetRequiredService<IEventStore>();
        return await eventStore.GetStreamVersionAsync(StreamId);
    }
}

/// <summary>
/// DTO for relationship with resolved names.
/// </summary>
public class RelationshipDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    public RelationshipType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
}

/// <summary>
/// Request model for creating a relationship.
/// </summary>
public class CreateRelationshipRequest
{
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    public RelationshipType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
}

/// <summary>
/// Request model for updating a relationship.
/// </summary>
public class UpdateRelationshipRequest
{
    public RelationshipType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsActive { get; set; }
}