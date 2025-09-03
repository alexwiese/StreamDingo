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
    private readonly IEventStreamManager<DomainSnapshot> _streamManager;
    private const string StreamId = "domain-stream";

    public UsersController(IEventStreamManager<DomainSnapshot> streamManager)
    {
        _streamManager = streamManager;
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        return Ok(snapshot?.ActiveUsers ?? Enumerable.Empty<User>());
    }

    /// <summary>
    /// Gets a specific user by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Users.TryGetValue(id, out var user) == true && !user.IsDeleted)
        {
            return Ok(user);
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
        var @event = new UserCreated
        {
            UserId = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Version = 0 // This will be set by the event store
        };

        var currentVersion = await GetCurrentVersionAsync();
        var snapshot = await _streamManager.AppendEventAsync(StreamId, @event, currentVersion);
        var user = snapshot?.Users.GetValueOrDefault(userId);
        
        return user != null ? CreatedAtAction(nameof(GetUser), new { id = userId }, user) : StatusCode(500);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<User>> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Users.TryGetValue(id, out var existingUser) != true || existingUser.IsDeleted)
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

        var currentVersion = await GetCurrentVersionAsync();
        var updatedSnapshot = await _streamManager.AppendEventAsync(StreamId, @event, currentVersion);
        var updatedUser = updatedSnapshot?.Users.GetValueOrDefault(id);
        
        return updatedUser != null ? Ok(updatedUser) : StatusCode(500);
    }

    /// <summary>
    /// Deletes a user (soft delete).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        if (snapshot?.Users.TryGetValue(id, out var existingUser) != true || existingUser.IsDeleted)
        {
            return NotFound();
        }

        var @event = new UserDeleted
        {
            UserId = id,
            Version = 0 // This will be set by the event store
        };

        var currentVersion = await GetCurrentVersionAsync();
        await _streamManager.AppendEventAsync(StreamId, @event, currentVersion);
        return NoContent();
    }

    private async Task<long> GetCurrentVersionAsync()
    {
        // Get the current stream version (-1 if stream doesn't exist)
        var eventStore = HttpContext.RequestServices.GetRequiredService<IEventStore>();
        return await eventStore.GetStreamVersionAsync(StreamId);
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