using System.Collections.Concurrent;

namespace StreamDingo;

/// <summary>
/// In-memory implementation of <see cref="ISnapshotStore{TSnapshot}"/> for development and testing.
/// This implementation is thread-safe but data is not persisted between application runs.
/// </summary>
/// <typeparam name="TSnapshot">The type of snapshot data.</typeparam>
public class InMemorySnapshotStore<TSnapshot> : ISnapshotStore<TSnapshot> where TSnapshot : class
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<long, Snapshot<TSnapshot>>> _snapshots = new();

    /// <inheritdoc />
    public Task SaveSnapshotAsync(string streamId, Snapshot<TSnapshot> snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        ArgumentNullException.ThrowIfNull(snapshot);

        var streamSnapshots = _snapshots.GetOrAdd(streamId, _ => new ConcurrentDictionary<long, Snapshot<TSnapshot>>());
        streamSnapshots.TryAdd(snapshot.Version, snapshot);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Snapshot<TSnapshot>?> GetLatestSnapshotAsync(string streamId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);

        if (!_snapshots.TryGetValue(streamId, out var streamSnapshots) || streamSnapshots.IsEmpty)
        {
            return Task.FromResult<Snapshot<TSnapshot>?>(null);
        }

        long maxVersion = streamSnapshots.Keys.Max();
        var snapshot = streamSnapshots.TryGetValue(maxVersion, out var result) ? result : null;

        return Task.FromResult(snapshot);
    }

    /// <inheritdoc />
    public Task<Snapshot<TSnapshot>?> GetSnapshotAtVersionAsync(string streamId, long maxVersion, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);

        if (!_snapshots.TryGetValue(streamId, out var streamSnapshots) || streamSnapshots.IsEmpty)
        {
            return Task.FromResult<Snapshot<TSnapshot>?>(null);
        }

        // Find the highest version that is <= maxVersion
        var validVersions = streamSnapshots.Keys.Where(v => v <= maxVersion).ToList();
        if (validVersions.Count == 0)
        {
            return Task.FromResult<Snapshot<TSnapshot>?>(null);
        }

        long targetVersion = validVersions.Max();
        var snapshot = streamSnapshots.TryGetValue(targetVersion, out var result) ? result : null;

        return Task.FromResult(snapshot);
    }

    /// <inheritdoc />
    public Task DeleteSnapshotsAsync(string streamId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        _snapshots.TryRemove(streamId, out _);
        return Task.CompletedTask;
    }
}
