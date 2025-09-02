using System.ComponentModel.DataAnnotations;

namespace StreamDingo.Core.Models;

/// <summary>
/// Represents a streaming session in the StreamDingo platform
/// </summary>
public class Stream
{
    /// <summary>
    /// Unique identifier for the stream
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Title of the stream
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the stream content
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// User who owns this stream
    /// </summary>
    [Required]
    public string StreamerId { get; set; } = string.Empty;

    /// <summary>
    /// Stream category/game being played
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Number of current viewers
    /// </summary>
    public int ViewerCount { get; set; } = 0;

    /// <summary>
    /// Whether the stream is currently live
    /// </summary>
    public bool IsLive { get; set; } = false;

    /// <summary>
    /// When the stream started
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// When the stream ended
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Stream quality settings
    /// </summary>
    public StreamQuality Quality { get; set; } = StreamQuality.Medium;

    /// <summary>
    /// Tags associated with the stream for discoverability
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Whether the stream is suitable for all audiences
    /// </summary>
    public bool IsFamilyFriendly { get; set; } = true;

    // TODO: Add methods for stream management
    // Copilot will help suggest useful methods here
}
