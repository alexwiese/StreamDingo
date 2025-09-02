namespace StreamDingo.Core.Models;

/// <summary>
/// Represents the quality levels available for streaming
/// </summary>
public enum StreamQuality
{
    /// <summary>
    /// Low quality - 480p, suitable for slow connections
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium quality - 720p, balanced quality and bandwidth
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High quality - 1080p, high bandwidth required
    /// </summary>
    High = 2,

    /// <summary>
    /// Ultra high quality - 4K, premium streaming
    /// </summary>
    Ultra = 3
}