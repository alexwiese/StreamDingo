---
Layout: _Layout
Title: custom-providers
---
# Custom Storage Providers

Learn how to implement custom storage providers for StreamDingo.

## Implementation Guide

### Event Store Provider
Implement the `IEventStore` interface:

```csharp
public interface IEventStore
{
    Task AppendEventsAsync(string streamId, IEnumerable<object> events);
    Task<IEnumerable<object>> GetEventsAsync(string streamId, long fromVersion = 0);
}
```

### Snapshot Store Provider
Implement the `ISnapshotStore` interface:

```csharp
public interface ISnapshotStore
{
    Task SaveSnapshotAsync<T>(string streamId, Snapshot<T> snapshot);
    Task<Snapshot<T>?> GetSnapshotAsync<T>(string streamId);
}
```

## Best Practices

1. Implement proper error handling
2. Use connection pooling for database providers
3. Consider transaction boundaries
4. Implement retry policies for transient failures
5. Add comprehensive logging

## Examples

Complete examples and detailed implementation guides coming soon.
