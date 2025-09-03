using Microsoft.AspNetCore.Mvc;
using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Models;

namespace StreamDingo.Examples.WebApp.Controllers;

/// <summary>
/// API controller for managing users.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IEventStreamManager<UserAggregate> _streamManager;

    public UsersController(IEventStreamManager<UserAggregate> streamManager)
    {
        _streamManager = streamManager;
    }

    /// <summary>
    /// Gets all users - this is a simplified implementation.
    /// In a real system, you'd typically have a read model or query all user streams.
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        // Note: This is a simplified implementation for demo purposes.
        // In a production system, you'd typically maintain a read model 
        // or use a different approach to list all users.
        return Ok(new List<User>());
    }

    /// <summary>
    /// Gets a specific user by ID using their individual stream.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var streamId = $"user-{id}";
        var userAggregate = await _streamManager.GetCurrentStateAsync(streamId);
        
        if (userAggregate?.ActiveUser != null)
        {
            return Ok(userAggregate.ActiveUser);
        }
        return NotFound();
    }

    /// <summary>
    /// Gets relationships for a specific user.
    /// </summary>
    [HttpGet("{id:guid}/relationships")]
    public async Task<ActionResult<IEnumerable<Relationship>>> GetUserRelationships(Guid id)
    {
        var streamId = $"user-{id}";
        var userAggregate = await _streamManager.GetCurrentStateAsync(streamId);
        
        if (userAggregate?.ActiveUser != null)
        {
            return Ok(userAggregate.ActiveRelationships);
        }
        return NotFound();
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(CreateUserRequest request)
    {
        var userId = Guid.NewGuid();
        var streamId = $"user-{userId}";
        
        var @event = new UserCreated
        {
            UserId = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Version = 0 // This will be set by the event store
        };

        var currentVersion = await GetStreamVersionAsync(streamId);
        var userAggregate = await _streamManager.AppendEventAsync(streamId, @event, currentVersion);
        var user = userAggregate?.ActiveUser;
        
        return user != null ? CreatedAtAction(nameof(GetUser), new { id = userId }, user) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<User>> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var streamId = $"user-{id}";
        var userAggregate = await _streamManager.GetCurrentStateAsync(streamId);
        
        if (userAggregate?.ActiveUser == null)
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

        var currentVersion = await GetStreamVersionAsync(streamId);
        var updatedAggregate = await _streamManager.AppendEventAsync(streamId, @event, currentVersion);
        var updatedUser = updatedAggregate?.ActiveUser;
        
        return updatedUser != null ? Ok(updatedUser) : StatusCode(500);
    }

    /// <summary>
    /// Deletes a user (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var streamId = $"user-{id}";
        var userAggregate = await _streamManager.GetCurrentStateAsync(streamId);
        
        if (userAggregate?.ActiveUser == null)
        {
            return NotFound();
        }

        var @event = new UserDeleted
        {
            UserId = id,
            Version = 0 // This will be set by the event store
        };

        var currentVersion = await GetStreamVersionAsync(streamId);
        await _streamManager.AppendEventAsync(streamId, @event, currentVersion);
        return NoContent();
    }

    private async Task<long> GetStreamVersionAsync(string streamId)
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