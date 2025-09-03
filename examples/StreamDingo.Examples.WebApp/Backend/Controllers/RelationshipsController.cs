using Microsoft.AspNetCore.Mvc;
using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Controllers;

/// <summary>
/// API controller for managing relationships with aggregate-based event sourcing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RelationshipsController : ControllerBase
{
    private readonly IEventStreamManager<UserAggregate> _userStreamManager;
    private readonly IEventStreamManager<BusinessAggregate> _businessStreamManager;
    private readonly IEventStreamManager<DomainSnapshot> _domainStreamManager;

    public RelationshipsController(
        IEventStreamManager<UserAggregate> userStreamManager,
        IEventStreamManager<BusinessAggregate> businessStreamManager,
        IEventStreamManager<DomainSnapshot> domainStreamManager)
    {
        _userStreamManager = userStreamManager;
        _businessStreamManager = businessStreamManager;
        _domainStreamManager = domainStreamManager;
    }

    /// <summary>
    /// Gets all relationships (using legacy domain stream for now).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetRelationships()
    {
        // For listing all relationships, we still use the domain stream
        // In a real system, this might be a projection/read model
        const string domainStreamId = "domain-stream";
        var snapshot = await _domainStreamManager.GetCurrentStateAsync(domainStreamId);
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
    /// Gets a specific relationship by ID (using legacy domain stream).
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RelationshipDto>> GetRelationship(Guid id)
    {
        const string domainStreamId = "domain-stream";
        var snapshot = await _domainStreamManager.GetCurrentStateAsync(domainStreamId);
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
    /// Gets relationships for a specific user (using aggregate-based querying).
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetUserRelationships(Guid userId)
    {
        // Use user aggregate for efficient querying
        var userStreamId = $"user-{userId}";
        var userAggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        
        if (userAggregate?.HasUser == true)
        {
            var relationshipDtos = userAggregate.ActiveRelationships.Select(r => new RelationshipDto
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
                UserName = userAggregate.User?.FullName ?? "Unknown",
                BusinessName = "Unknown" // Would need to query business aggregate for name
            });
            
            return Ok(relationshipDtos);
        }
        return NotFound($"User {userId} not found.");
    }

    /// <summary>
    /// Gets relationships for a specific business (using aggregate-based querying).
    /// </summary>
    [HttpGet("business/{businessId:guid}")]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetBusinessRelationships(Guid businessId)
    {
        // Use business aggregate for efficient querying
        var businessStreamId = $"business-{businessId}";
        var businessAggregate = await _businessStreamManager.GetCurrentStateAsync(businessStreamId);
        
        if (businessAggregate?.HasBusiness == true)
        {
            var relationshipDtos = businessAggregate.ActiveRelationships.Select(r => new RelationshipDto
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
                UserName = "Unknown", // Would need to query user aggregate for name
                BusinessName = businessAggregate.Business?.Name ?? "Unknown"
            });
            
            return Ok(relationshipDtos);
        }
        return NotFound($"Business {businessId} not found.");
    }

    /// <summary>
    /// Creates a new relationship using aggregate-based event sourcing.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RelationshipDto>> CreateRelationship(CreateRelationshipRequest request)
    {
        const string domainStreamId = "domain-stream";
        var userStreamId = $"user-{request.UserId}";
        var businessStreamId = $"business-{request.BusinessId}";

        // Validate that user and business exist using their aggregates
        var userAggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        var businessAggregate = await _businessStreamManager.GetCurrentStateAsync(businessStreamId);
        
        if (userAggregate?.HasUser != true || businessAggregate?.HasBusiness != true)
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

        // Append to all three streams for proper aggregate maintenance
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        var userCurrentVersion = await GetCurrentVersionAsync(userStreamId);
        var businessCurrentVersion = await GetCurrentVersionAsync(businessStreamId);
        
        var domainSnapshot = await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        await _userStreamManager.AppendEventAsync(userStreamId, @event, userCurrentVersion);
        await _businessStreamManager.AppendEventAsync(businessStreamId, @event, businessCurrentVersion);
        
        var relationship = domainSnapshot?.Relationships.GetValueOrDefault(relationshipId);
        
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
                UserName = userAggregate.User?.FullName ?? "Unknown",
                BusinessName = businessAggregate.Business?.Name ?? "Unknown"
            };
            
            return CreatedAtAction(nameof(GetRelationship), new { id = relationshipId }, relationshipDto);
        }
        
        return StatusCode(500);
    }

    /// <summary>
    /// Updates an existing relationship using aggregate-based event sourcing.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RelationshipDto>> UpdateRelationship(Guid id, UpdateRelationshipRequest request)
    {
        const string domainStreamId = "domain-stream";
        
        // First find which relationship we're updating from the domain stream
        var domainSnapshot = await _domainStreamManager.GetCurrentStateAsync(domainStreamId);
        if (domainSnapshot?.Relationships.TryGetValue(id, out var existingRelationship) != true || existingRelationship.IsDeleted)
        {
            return NotFound();
        }

        var userStreamId = $"user-{existingRelationship.UserId}";
        var businessStreamId = $"business-{existingRelationship.BusinessId}";
        
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

        // Append to all three streams
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        var userCurrentVersion = await GetCurrentVersionAsync(userStreamId);
        var businessCurrentVersion = await GetCurrentVersionAsync(businessStreamId);
        
        var updatedDomainSnapshot = await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        await _userStreamManager.AppendEventAsync(userStreamId, @event, userCurrentVersion);
        await _businessStreamManager.AppendEventAsync(businessStreamId, @event, businessCurrentVersion);
        
        var updatedRelationship = updatedDomainSnapshot?.Relationships.GetValueOrDefault(id);
        
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
                UserName = updatedDomainSnapshot.Users.GetValueOrDefault(updatedRelationship.UserId)?.FullName ?? "Unknown",
                BusinessName = updatedDomainSnapshot.Businesses.GetValueOrDefault(updatedRelationship.BusinessId)?.Name ?? "Unknown"
            };
            
            return Ok(relationshipDto);
        }
        
        return StatusCode(500);
    }

    /// <summary>
    /// Deletes a relationship (soft delete) using aggregate-based event sourcing.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteRelationship(Guid id)
    {
        const string domainStreamId = "domain-stream";
        
        // First find which relationship we're deleting from the domain stream
        var domainSnapshot = await _domainStreamManager.GetCurrentStateAsync(domainStreamId);
        if (domainSnapshot?.Relationships.TryGetValue(id, out var existingRelationship) != true || existingRelationship.IsDeleted)
        {
            return NotFound();
        }

        var userStreamId = $"user-{existingRelationship.UserId}";
        var businessStreamId = $"business-{existingRelationship.BusinessId}";
        
        var @event = new RelationshipDeleted
        {
            RelationshipId = id,
            Version = 0
        };

        // Append to all three streams
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        var userCurrentVersion = await GetCurrentVersionAsync(userStreamId);
        var businessCurrentVersion = await GetCurrentVersionAsync(businessStreamId);
        
        await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        await _userStreamManager.AppendEventAsync(userStreamId, @event, userCurrentVersion);
        await _businessStreamManager.AppendEventAsync(businessStreamId, @event, businessCurrentVersion);
        
        return NoContent();
    }

    private async Task<long> GetCurrentVersionAsync(string streamId)
    {
        var eventStore = HttpContext.RequestServices.GetRequiredService<IEventStore>();
        return await eventStore.GetStreamVersionAsync(streamId);
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