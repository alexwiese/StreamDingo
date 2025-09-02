# Event Replay

Event replay is the process of reconstructing aggregate state by reapplying stored events. StreamDingo provides intelligent replay strategies optimized for performance and integrity.

## Replay Triggers

### Handler Code Changes
When event handler code changes, affected streams need replay:

```csharp
// Handler change detected - replay required
var currentHash = hashProvider.HashEventHandlers<UserAggregate>();
if (currentHash != snapshot.Metadata.AggregateHash)
{
    await TriggerReplayAsync<UserAggregate>(streamId);
}
```

### Data Integrity Issues
Corrupted snapshots trigger replay from last valid state:

```csharp
var snapshotValid = await VerifySnapshotIntegrityAsync(streamId);
if (!snapshotValid)
{
    await ReplayFromLastValidSnapshotAsync(streamId);
}
```

## Replay Strategies

### Full Replay
Replay all events from the beginning:

```csharp
public async Task<TAggregate?> FullReplayAsync<TAggregate>(string streamId)
{
    var events = await eventStore.GetAllEventsAsync(streamId);
    return events.Aggregate(default(TAggregate), ApplyEventToState);
}
```

### Snapshot-Based Replay
Replay from the most recent valid snapshot:

```csharp
public async Task<TAggregate?> SnapshotBasedReplayAsync<TAggregate>(string streamId)
{
    var snapshot = await GetLatestValidSnapshotAsync<TAggregate>(streamId);
    var events = await eventStore.GetEventsFromVersionAsync(streamId, snapshot.Version + 1);
    
    return events.Aggregate(snapshot.State, ApplyEventToState);
}
```

## Performance Optimization

### Batch Processing
Process events in batches for better performance:

```csharp
public async Task<TAggregate?> BatchReplayAsync<TAggregate>(string streamId, int batchSize = 1000)
{
    var state = default(TAggregate);
    var version = 0L;
    
    while (true)
    {
        var events = await eventStore.GetEventBatchAsync(streamId, version, batchSize);
        if (!events.Any()) break;
        
        foreach (var @event in events)
        {
            state = ApplyEventToState(state, @event);
        }
        
        version = events.Last().Version + 1;
    }
    
    return state;
}
```

### Parallel Replay
For multiple streams, use parallel processing:

```csharp
public async Task<Dictionary<string, TAggregate>> ReplayMultipleStreamsAsync<TAggregate>(
    IEnumerable<string> streamIds)
{
    var tasks = streamIds.Select(async streamId =>
    {
        var state = await ReplayEventsAsync<TAggregate>(streamId);
        return new KeyValuePair<string, TAggregate>(streamId, state);
    });
    
    var results = await Task.WhenAll(tasks);
    return results.ToDictionary(r => r.Key, r => r.Value);
}
```

## Next Steps

- Learn about [Storage Providers](storage-providers.md)
- Explore [Performance](../advanced/performance.md) optimization