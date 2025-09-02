using System.ComponentModel.DataAnnotations;

namespace StreamDingo.Core.Models;

/// <summary>
/// Represents a user in the StreamDingo platform
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Username for the user (must be unique)
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the user
    /// </summary>
    [StringLength(100)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// User's email address
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's biography or description
    /// </summary>
    [StringLength(500)]
    public string? Biography { get; set; }

    /// <summary>
    /// URL to user's profile picture
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// When the user account was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the user last logged in
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Whether the user account is verified
    /// </summary>
    public bool IsVerified { get; set; } = false;

    /// <summary>
    /// Whether the user can stream content
    /// </summary>
    public bool CanStream { get; set; } = true;

    /// <summary>
    /// Number of followers this user has
    /// </summary>
    public int FollowerCount { get; set; } = 0;

    /// <summary>
    /// Number of users this user is following
    /// </summary>
    public int FollowingCount { get; set; } = 0;

    // TODO: Add user management methods
    // Copilot will suggest useful methods for user operations
}