using System.Collections.Concurrent;

namespace StreamDingo;

/// <summary>
/// In-memory implementation of <see cref="IEventStore"/> for development and testing.
/// This implementation is thread-safe but data is not persisted between application runs.
/// </summary>
public class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<long, IEvent>> _streams = new();
    private readonly ConcurrentDictionary<string, long> _streamVersions = new();

    /// <inheritdoc />
    public Task AppendEventAsync(string streamId, IEvent @event, long expectedVersion, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        ArgumentNullException.ThrowIfNull(@event);

        var stream = _streams.GetOrAdd(streamId, _ => new ConcurrentDictionary<long, IEvent>());
        long currentVersion = _streamVersions.GetOrAdd(streamId, -1);

        if (currentVersion != expectedVersion)
        {
            throw new ConcurrencyException(streamId, expectedVersion, currentVersion);
        }

        long newVersion = currentVersion + 1;

        // Store the original event, but track the version separately
        stream.TryAdd(newVersion, @event);
        _streamVersions.TryUpdate(streamId, newVersion, currentVersion);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AppendEventsAsync(string streamId, IEnumerable<IEvent> events, long expectedVersion, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        ArgumentNullException.ThrowIfNull(events);

        var eventList = events.ToList();
        if (eventList.Count == 0)
        {
            return Task.CompletedTask;
        }

        var stream = _streams.GetOrAdd(streamId, _ => new ConcurrentDictionary<long, IEvent>());
        long currentVersion = _streamVersions.GetOrAdd(streamId, -1);

        if (currentVersion != expectedVersion)
        {
            throw new ConcurrencyException(streamId, expectedVersion, currentVersion);
        }

        long newVersion = currentVersion;
        foreach (var @event in eventList)
        {
            newVersion++;
            // Store the original event, but track the version separately
            stream.TryAdd(newVersion, @event);
        }

        _streamVersions.TryUpdate(streamId, newVersion, currentVersion);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IEvent> ReadEventsAsync(string streamId, long fromVersion = 0, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);

        if (!_streams.TryGetValue(streamId, out var stream))
        {
            yield break;
        }

        long currentVersion = _streamVersions.GetOrAdd(streamId, -1);

        for (long version = Math.Max(0, fromVersion); version <= currentVersion; version++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (stream.TryGetValue(version, out var @event))
            {
                // Return the original event - event handlers should work with the original type
                yield return @event;
            }

            // Allow for cancellation and yielding control
            if (version % 100 == 0)
            {
                await Task.Yield();
            }
        }
    }

    /// <inheritdoc />
    public Task<long> GetStreamVersionAsync(string streamId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        return Task.FromResult(_streamVersions.GetOrAdd(streamId, -1));
    }

    /// <inheritdoc />
    public Task<bool> StreamExistsAsync(string streamId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamId);
        return Task.FromResult(_streams.ContainsKey(streamId));
    }
}
