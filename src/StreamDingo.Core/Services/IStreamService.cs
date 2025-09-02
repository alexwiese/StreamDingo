using StreamDingo.Core.Models;

namespace StreamDingo.Core.Services;

/// <summary>
/// Service interface for managing streaming operations
/// This interface demonstrates patterns that GitHub Copilot can help implement
/// </summary>
public interface IStreamService
{
    /// <summary>
    /// Creates a new stream for a user
    /// </summary>
    /// <param name="streamerId">ID of the user creating the stream</param>
    /// <param name="title">Title of the stream</param>
    /// <param name="description">Optional description of the stream</param>
    /// <param name="category">Stream category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created stream</returns>
    Task<Models.Stream> CreateStreamAsync(string streamerId, string title, string? description = null, 
        string? category = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a stream (makes it live)
    /// </summary>
    /// <param name="streamId">ID of the stream to start</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated stream</returns>
    Task<Models.Stream> StartStreamAsync(Guid streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends a stream
    /// </summary>
    /// <param name="streamId">ID of the stream to end</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated stream</returns>
    Task<Models.Stream> EndStreamAsync(Guid streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active (live) streams
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active streams</returns>
    Task<IEnumerable<Models.Stream>> GetActiveStreamsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets streams by category
    /// </summary>
    /// <param name="category">Category to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Streams in the specified category</returns>
    Task<IEnumerable<Models.Stream>> GetStreamsByCategoryAsync(string category, 
        CancellationToken cancellationToken = default);

    // TODO: Add more methods for stream management
    // Copilot can suggest additional useful methods based on the streaming domain:
    // - GetPopularStreamsAsync
    // - SearchStreamsAsync 
    // - UpdateStreamMetadataAsync
    // - GetStreamViewersAsync
    // - BanViewerFromStreamAsync
}