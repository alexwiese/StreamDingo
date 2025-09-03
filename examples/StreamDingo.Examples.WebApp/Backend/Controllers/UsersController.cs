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
            Version = await GetNextVersionAsync()
        };

        var snapshot = await _streamManager.AppendEventAsync(StreamId, @event, @event.Version - 1);
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
            Version = await GetNextVersionAsync()
        };

        var updatedSnapshot = await _streamManager.AppendEventAsync(StreamId, @event, @event.Version - 1);
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
            Version = await GetNextVersionAsync()
        };

        await _streamManager.AppendEventAsync(StreamId, @event, @event.Version - 1);
        return NoContent();
    }

    private async Task<long> GetNextVersionAsync()
    {
        // Simple version incrementing - in a real app you'd want more sophisticated versioning
        var snapshot = await _streamManager.GetCurrentStateAsync(StreamId);
        return (snapshot?.Users.Count + snapshot?.Businesses.Count + snapshot?.Relationships.Count + 1) ?? 1;
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