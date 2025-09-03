using Microsoft.AspNetCore.Mvc;
using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Controllers;

/// <summary>
/// API controller for managing users with aggregate-based event sourcing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IEventStreamManager<UserAggregate> _userStreamManager;
    private readonly IEventStreamManager<DomainSnapshot> _domainStreamManager;

    public UsersController(
        IEventStreamManager<UserAggregate> userStreamManager,
        IEventStreamManager<DomainSnapshot> domainStreamManager)
    {
        _userStreamManager = userStreamManager;
        _domainStreamManager = domainStreamManager;
    }

    /// <summary>
    /// Gets all users (using legacy domain stream for now).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        // For listing all users, we still use the domain stream
        // In a real system, this might be a projection/read model
        const string domainStreamId = "domain-stream";
        var snapshot = await _domainStreamManager.GetCurrentStateAsync(domainStreamId);
        return Ok(snapshot?.ActiveUsers ?? Enumerable.Empty<User>());
    }

    /// <summary>
    /// Gets a specific user by ID using aggregate-based querying.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var streamId = $"user-{id}";
        var aggregate = await _userStreamManager.GetCurrentStateAsync(streamId);
        
        if (aggregate?.HasUser == true)
        {
            return Ok(aggregate.User);
        }
        return NotFound();
    }

    /// <summary>
    /// Gets relationships for a specific user using aggregate-based querying.
    /// </summary>
    [HttpGet("{id:guid}/relationships")]
    public async Task<ActionResult<IEnumerable<Relationship>>> GetUserRelationships(Guid id)
    {
        var streamId = $"user-{id}";
        var aggregate = await _userStreamManager.GetCurrentStateAsync(streamId);
        
        if (aggregate?.HasUser == true)
        {
            return Ok(aggregate.ActiveRelationships);
        }
        return NotFound();
    }

    /// <summary>
    /// Creates a new user using aggregate-based event sourcing.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
    {
        var userId = Guid.NewGuid();
        var userStreamId = $"user-{userId}";
        var domainStreamId = "domain-stream";
        
        var @event = new UserCreated
        {
            UserId = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Version = 0 // This will be set by the event store
        };

        // Append to both aggregate stream and domain stream (for backward compatibility and listing)
        var userCurrentVersion = await GetCurrentVersionAsync(userStreamId);
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        
        var userAggregate = await _userStreamManager.AppendEventAsync(userStreamId, @event, userCurrentVersion);
        await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        
        var user = userAggregate?.User;
        return user != null ? CreatedAtAction(nameof(GetUser), new { id = userId }, user) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing user using aggregate-based event sourcing.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<User>> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var userStreamId = $"user-{id}";
        var domainStreamId = "domain-stream";
        
        // Check if user exists in aggregate
        var aggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        if (aggregate?.HasUser != true)
        {
            return NotFound();
        }

        var @event = new UserUpdated
        {
            UserId = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Version = 0 // This will be set by the event store
        };

        // Append to both aggregate stream and domain stream
        var userCurrentVersion = await GetCurrentVersionAsync(userStreamId);
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        
        var updatedAggregate = await _userStreamManager.AppendEventAsync(userStreamId, @event, userCurrentVersion);
        await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        
        var updatedUser = updatedAggregate?.User;
        return updatedUser != null ? Ok(updatedUser) : StatusCode(500);
    }

    /// <summary>
    /// Deletes a user (soft delete) using aggregate-based event sourcing.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var userStreamId = $"user-{id}";
        var domainStreamId = "domain-stream";
        
        // Check if user exists in aggregate
        var aggregate = await _userStreamManager.GetCurrentStateAsync(userStreamId);
        if (aggregate?.HasUser != true)
        {
            return NotFound();
        }

        var @event = new UserDeleted
        {
            UserId = id,
            Version = 0 // This will be set by the event store
        };

        // Append to both aggregate stream and domain stream
        var userCurrentVersion = await GetCurrentVersionAsync(userStreamId);
        var domainCurrentVersion = await GetCurrentVersionAsync(domainStreamId);
        
        await _userStreamManager.AppendEventAsync(userStreamId, @event, userCurrentVersion);
        await _domainStreamManager.AppendEventAsync(domainStreamId, @event, domainCurrentVersion);
        
        return NoContent();
    }

    private async Task<long> GetCurrentVersionAsync(string streamId)
    {
        // Get the current stream version (-1 if stream doesn't exist)
        var eventStore = HttpContext.RequestServices.GetRequiredService<IEventStore>();
        return await eventStore.GetStreamVersionAsync(streamId);
    }
}

/// <summary>
/// Request model for creating a user.
/// </summary>
public class CreateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating a user.
/// </summary>
public class UpdateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}