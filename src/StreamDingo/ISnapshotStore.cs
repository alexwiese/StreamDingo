namespace StreamDingo;

/// <summary>
/// Provides storage and retrieval functionality for snapshots.
/// </summary>
/// <typeparam name="TSnapshot">The type of snapshot data.</typeparam>
public interface ISnapshotStore<TSnapshot> where TSnapshot : class
{
    /// <summary>
    /// Saves a snapshot to the store.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="snapshot">The snapshot to save.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SaveSnapshotAsync(string streamId, Snapshot<TSnapshot> snapshot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent snapshot for the specified stream.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The most recent snapshot, or null if no snapshots exist.</returns>
    public Task<Snapshot<TSnapshot>?> GetLatestSnapshotAsync(string streamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a snapshot at or before the specified version.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="maxVersion">The maximum version to consider.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The snapshot at or before the specified version, or null if none exists.</returns>
    public Task<Snapshot<TSnapshot>?> GetSnapshotAtVersionAsync(string streamId, long maxVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all snapshots for the specified stream.
    /// </summary>
    /// <param name="streamId">The identifier of the event stream.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteSnapshotsAsync(string streamId, CancellationToken cancellationToken = default);
}
