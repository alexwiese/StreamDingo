---
Layout: _Layout
Title: storage-providers
---
# Storage Providers

StreamDingo supports multiple storage providers for events and snapshots, allowing you to choose the best backend for your needs.

## Available Providers

### In-Memory Storage
Perfect for development, testing, and prototyping:

```csharp
services.AddSingleton<IEventStore, InMemoryEventStore>();
services.AddSingleton<ISnapshotStore, InMemorySnapshotStore>();
```

### SQL Server Provider
Production-ready relational storage:

```csharp
services.AddSqlServerEventStore(connectionString);
services.AddSqlServerSnapshotStore(connectionString);
```

### PostgreSQL Provider
Open-source relational storage with JSONB support:

```csharp
services.AddPostgreSqlEventStore(connectionString);
services.AddPostgreSqlSnapshotStore(connectionString);
```

## Configuration

### Connection Strings
```csharp
services.Configure<StorageOptions>(options =>
{
    options.EventStoreConnectionString = "Server=...;Database=Events;";
    options.SnapshotStoreConnectionString = "Server=...;Database=Snapshots;";
    options.EnableConnectionPooling = true;
    options.CommandTimeout = TimeSpan.FromSeconds(30);
});
```

### Performance Tuning
```csharp
services.Configure<EventStoreOptions>(options =>
{
    options.BatchSize = 1000;
    options.EnableBulkInsert = true;
    options.UseCompression = true;
});
```

## Custom Providers

Implement `IEventStore` and `ISnapshotStore` for custom storage:

```csharp
public class CustomEventStore : IEventStore
{
    public async Task AppendEventsAsync(string streamId, IEnumerable<object> events)
    {
        // Custom implementation
    }
    
    public async Task<IEnumerable<object>> GetEventsAsync(string streamId, long fromVersion = 0)
    {
        // Custom implementation
    }
}
```

## Next Steps

- Learn about [Custom Providers](../advanced/custom-providers.html)
- Explore [Performance](../advanced/performance.html) optimization
