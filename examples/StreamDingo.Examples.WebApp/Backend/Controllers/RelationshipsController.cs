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
    private readonly IEventStreamManager<UserAggregate> _userStreamManager;
    private readonly IEventStreamManager<BusinessAggregate> _businessStreamManager;

    public RelationshipsController(
        IEventStreamManager<UserAggregate> userStreamManager,
        IEventStreamManager<BusinessAggregate> businessStreamManager)
    {
        _userStreamManager = userStreamManager;
        _businessStreamManager = businessStreamManager;
    }

    /// <summary>
    /// Gets all relationships - simplified implementation.
    /// In a production system, you'd typically have a read model for cross-aggregate queries.
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<RelationshipDto>> GetRelationships()
    {
        // Note: This is a simplified implementation for demo purposes.
        // In a production system, you'd typically maintain a read model 
        // for cross-aggregate queries like "get all relationships".
        return Ok(new List<RelationshipDto>());
    }

    /// <summary>
    /// Gets a specific relationship by ID.
    /// Note: Since we don't know which user stream contains the relationship,
    /// this would typically require a read model in a production system.
    /// </summary>
    [HttpGet("{id:guid}")]
    public ActionResult<RelationshipDto> GetRelationship(Guid id)
    {
        // In a production system, you'd use a read model to find which user
        // stream contains this relationship, then query that specific stream.
        return NotFound("Individual relationship lookup requires knowing the user ID. Use /api/users/{userId}/relationships instead.");
    }

    /// <summary>
    /// Gets relationships for a specific user using their individual stream.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<RelationshipDto>>> GetUserRelationships(Guid userId)
    {
        var userStreamId = $"user-{userId}";
        var userAggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        
        if (userAggregate?.ActiveUser == null)
        {
            return NotFound("User not found.");
        }

        var relationshipDtos = new List<RelationshipDto>();

        foreach (var relationship in userAggregate.ActiveRelationships)
        {
            // Get business name by looking up the business stream
            var businessStreamId = $"business-{relationship.BusinessId}";
            var businessAggregate = await _businessStreamManager.GetCurrentStateAsync(businessStreamId);
            
            relationshipDtos.Add(new RelationshipDto
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
                UserName = userAggregate.ActiveUser.FullName,
                BusinessName = businessAggregate?.ActiveBusiness?.Name ?? "Unknown"
            });
        }

        return Ok(relationshipDtos);
    }

    /// <summary>
    /// Gets relationships for a specific business.
    /// Note: This requires querying multiple user streams, so it's simplified here.
    /// In production, you'd use a read model for this type of query.
    /// </summary>
    [HttpGet("business/{businessId:guid}")]
    public ActionResult<IEnumerable<RelationshipDto>> GetBusinessRelationships(Guid businessId)
    {
        // In a production system, you'd maintain a read model that indexes
        // relationships by business ID, since relationships are stored in user streams.
        return Ok(new List<RelationshipDto>());
    }

    /// <summary>
    /// Creates a new relationship.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RelationshipDto>> CreateRelationship(CreateRelationshipRequest request)
    {
        // Validate that user exists by checking their stream
        var userStreamId = $"user-{request.UserId}";
        var userAggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        
        if (userAggregate?.ActiveUser == null)
        {
            return BadRequest("User not found or has been deleted.");
        }

        // Validate that business exists by checking their stream
        var businessStreamId = $"business-{request.BusinessId}";
        var businessAggregate = await _businessStreamManager.GetCurrentStateAsync(businessStreamId);
        
        if (businessAggregate?.ActiveBusiness == null)
        {
            return BadRequest("Business not found or has been deleted.");
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

        // Store the relationship in the user's stream
        var currentVersion = await GetStreamVersionAsync(userStreamId);
        var updatedUserAggregate = await _userStreamManager.AppendEventAsync(userStreamId, @event, currentVersion);
        var relationship = updatedUserAggregate?.Relationships.GetValueOrDefault(relationshipId);
        
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
                UserName = updatedUserAggregate.ActiveUser?.FullName ?? "Unknown",
                BusinessName = businessAggregate.ActiveBusiness.Name
            };
            
            return CreatedAtAction(nameof(GetUserRelationships), new { userId = request.UserId }, relationshipDto);
        }
        
        return StatusCode(500);
    }

    /// <summary>
    /// Updates an existing relationship.
    /// </summary>
    [HttpPut("{userId:guid}/{relationshipId:guid}")]
    public async Task<ActionResult<RelationshipDto>> UpdateRelationship(Guid userId, Guid relationshipId, UpdateRelationshipRequest request)
    {
        var userStreamId = $"user-{userId}";
        var userAggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        
        if (userAggregate?.Relationships.TryGetValue(relationshipId, out var existingRelationship) != true || 
            existingRelationship.IsDeleted)
        {
            return NotFound();
        }

        var @event = new RelationshipUpdated
        {
            RelationshipId = relationshipId,
            Type = request.Type,
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            Version = 0
        };

        var currentVersion = await GetStreamVersionAsync(userStreamId);
        var updatedUserAggregate = await _userStreamManager.AppendEventAsync(userStreamId, @event, currentVersion);
        var updatedRelationship = updatedUserAggregate?.Relationships.GetValueOrDefault(relationshipId);
        
        if (updatedRelationship != null)
        {
            // Get business name
            var businessStreamId = $"business-{updatedRelationship.BusinessId}";
            var businessAggregate = await _businessStreamManager.GetCurrentStateAsync(businessStreamId);
            
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
                UserName = updatedUserAggregate.ActiveUser?.FullName ?? "Unknown",
                BusinessName = businessAggregate?.ActiveBusiness?.Name ?? "Unknown"
            };
            
            return Ok(relationshipDto);
        }
        
        return StatusCode(500);
    }

    /// <summary>
    /// Deletes a relationship (soft delete).
    /// </summary>
    [HttpDelete("{userId:guid}/{relationshipId:guid}")]
    public async Task<ActionResult> DeleteRelationship(Guid userId, Guid relationshipId)
    {
        var userStreamId = $"user-{userId}";
        var userAggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        
        if (userAggregate?.Relationships.TryGetValue(relationshipId, out var existingRelationship) != true || 
            existingRelationship.IsDeleted)
        {
            return NotFound();
        }

        var @event = new RelationshipDeleted
        {
            RelationshipId = relationshipId,
            Version = 0
        };

        var currentVersion = await GetStreamVersionAsync(userStreamId);
        await _userStreamManager.AppendEventAsync(userStreamId, @event, currentVersion);
        return NoContent();
    }

    private async Task<long> GetStreamVersionAsync(string streamId)
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