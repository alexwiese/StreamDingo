---
---
# StreamDingo - API Design Principles

> **Guiding principles for consistent, performant, and user-friendly API design**

## 🎯 Core Philosophy

StreamDingo follows these fundamental principles in all API design decisions:

1. **Simplicity Over Flexibility**: Prefer simple, obvious APIs over complex, highly configurable ones
2. **Performance by Default**: Design for performance without requiring optimization expertise
3. **Fail Fast**: Validate early and provide clear error messages
4. **Async by Design**: All I/O operations are async with proper cancellation support
5. **Immutable State**: Prefer immutable objects and pure functions where possible

## 🔧 API Design Rules

### 1. Method Naming Conventions

#### Async Operations
- All async methods MUST end with `Async`
- Return `ValueTask<T>` for potentially synchronous operations
- Return `Task<T>` for always asynchronous operations
- Always accept `CancellationToken` as the last parameter

```csharp
// ✅ Good
ValueTask<Snapshot<T>?> GetLatestSnapshotAsync<T>(string streamId, CancellationToken cancellationToken = default);

// ❌ Bad  
Task<Snapshot<T>?> GetLatestSnapshot<T>(string streamId);
```

#### Sync Operations  
- Avoid synchronous I/O operations
- Use for pure functions and in-memory operations only
- Prefer `Span<T>` and `ReadOnlySpan<T>` for buffer operations

```csharp
// ✅ Good - Pure function
string CalculateHash(ReadOnlySpan<byte> data);

// ❌ Bad - I/O operation
Snapshot<T> GetLatestSnapshot<T>(string streamId);
```

### 2. Parameter Design

#### Required Parameters
- Required parameters come first
- Use descriptive parameter names
- Avoid `object` parameters unless absolutely necessary

#### Optional Parameters
- Use default parameter values where logical
- Group related optional parameters into options objects
- Use nullable types for truly optional values

```csharp
// ✅ Good
ValueTask AppendEventAsync<T>(string streamId, T @event, 
    EventAppendOptions? options = null, CancellationToken cancellationToken = default);

// ❌ Bad
ValueTask AppendEventAsync<T>(string streamId, T @event, bool validateHash = true, 
    int timeoutMs = 30000, bool createSnapshot = false, CancellationToken cancellationToken = default);
```

### 3. Generic Type Design

#### Type Constraints
- Use meaningful constraints that communicate intent
- Prefer interface constraints over class constraints
- Use `where T : class` for reference types that can be null

```csharp
// ✅ Good
public interface IEventHandler<TState, TEvent>
    where TState : class
    where TEvent : IEvent
{
    TState Handle(TState? previousSnapshot, TEvent @event);
}

// ❌ Bad
public interface IEventHandler<TState, TEvent>
{
    TState Handle(TState previousSnapshot, TEvent @event);
}
```

### 4. Exception Handling

#### Custom Exceptions
- Create specific exception types for different error scenarios
- Include relevant context in exception messages
- Use inner exceptions to preserve stack traces

```csharp
// ✅ Good
public class EventReplayException : Exception
{
    public string StreamId { get; }
    public long Version { get; }
    
    public EventReplayException(string streamId, long version, string message, Exception? innerException = null)
        : base($"Failed to replay events for stream '{streamId}' at version {version}: {message}", innerException)
    {
        StreamId = streamId;
        Version = version;
    }
}

// ❌ Bad
throw new Exception("Replay failed");
```

#### Error Communication
- Use Result<T, TError> pattern for expected failures
- Reserve exceptions for unexpected errors
- Provide retry guidance in error messages

### 5. Configuration and Options

#### Builder Pattern
- Use fluent builder APIs for complex configuration
- Provide reasonable defaults for all optional settings
- Validate configuration at build time, not runtime

```csharp
// ✅ Good
var eventSourcing = EventSourcingBuilder
    .Create()
    .UseInMemoryStore()
    .WithSnapshotInterval(100)
    .EnableHashVerification()
    .Build();

// ❌ Bad
var options = new EventSourcingOptions
{
    StoreType = EventStoreType.InMemory,
    SnapshotInterval = 100,
    HashVerificationEnabled = true
};
var eventSourcing = new EventSourcingEngine(options);
```

#### Options Classes
- Use record types for immutable options
- Group related settings together
- Provide validation methods

```csharp
// ✅ Good
public record EventSourcingOptions
{
    public int SnapshotInterval { get; init; } = 100;
    public bool EnableHashVerification { get; init; } = true;
    public TimeSpan EventTimeout { get; init; } = TimeSpan.FromSeconds(30);
    
    public void Validate()
    {
        if (SnapshotInterval <= 0)
            throw new ArgumentException("SnapshotInterval must be positive", nameof(SnapshotInterval));
    }
}
```

### 6. Dependency Injection Integration

#### Service Registration
- Provide extension methods for `IServiceCollection`
- Support both singleton and scoped lifetimes appropriately
- Allow configuration via `IConfiguration`

```csharp
// ✅ Good
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStreamDingo(this IServiceCollection services,
        Action<EventSourcingBuilder>? configure = null)
    {
        var builder = EventSourcingBuilder.Create();
        configure?.Invoke(builder);
        
        services.AddSingleton(builder.Build());
        return services;
    }
}
```

### 7. Serialization

#### Flexible Serialization
- Support multiple serialization providers
- Default to System.Text.Json for .NET objects
- Allow custom serializers for specific types

```csharp
// ✅ Good
public interface IEventSerializer
{
    ReadOnlyMemory<byte> Serialize<T>(T obj) where T : IEvent;
    T Deserialize<T>(ReadOnlyMemory<byte> data) where T : IEvent;
    object Deserialize(ReadOnlyMemory<byte> data, Type eventType);
}
```

### 8. Observability Integration

#### Logging
- Use `ILogger<T>` for all logging
- Provide structured logging with relevant context
- Use appropriate log levels

```csharp
// ✅ Good  
_logger.LogInformation("Event appended to stream {StreamId} at version {Version}", 
    streamId, version);

// ❌ Bad
_logger.LogInformation($"Event appended to stream {streamId} at version {version}");
```

#### Metrics
- Expose key metrics via .NET meters
- Use consistent naming conventions
- Provide both counter and histogram metrics

```csharp
// ✅ Good
public static class StreamDingoMetrics
{
    private static readonly Meter Meter = new("StreamDingo.EventSourcing");
    
    public static readonly Counter<long> EventsAppended = 
        Meter.CreateCounter<long>("streamdingo.events.appended");
    
    public static readonly Histogram<double> ReplayDuration = 
        Meter.CreateHistogram<double>("streamdingo.replay.duration", "ms");
}
```

## 🎯 Target Use Cases

### 1. Simple Event Sourcing
```csharp
// Minimal setup for basic event sourcing
var eventSourcing = EventSourcingBuilder
    .Create()
    .UseInMemoryStore()
    .Build();

await eventSourcing.AppendEventAsync("order-123", new OrderCreated("123", "John Doe"));
var currentState = await eventSourcing.GetCurrentStateAsync<OrderState>("order-123");
```

### 2. Production Event Sourcing
```csharp
// Full production setup with SQL Server and monitoring
var eventSourcing = EventSourcingBuilder
    .Create()
    .UseSqlServer(connectionString)
    .WithSnapshotInterval(500)
    .EnableHashVerification()
    .EnableTelemetry()
    .Build();
```

### 3. Custom Event Processing
```csharp
// Custom event handler registration
eventSourcing.RegisterHandler<OrderState, OrderCreated>(new OrderCreatedHandler());
eventSourcing.RegisterHandler<OrderState, OrderShipped>(new OrderShippedHandler());
```

## 🛡️ Backward Compatibility

### Versioning Strategy
- Follow Semantic Versioning (SemVer)
- Major version for breaking changes
- Minor version for new features
- Patch version for bug fixes

### API Evolution
- Mark obsolete APIs with `[Obsolete]` attribute
- Provide migration path in XML documentation
- Support old APIs for at least one major version
- Use `#pragma warning disable` sparingly

### Breaking Change Guidelines
- Avoid breaking changes in minor versions
- Document all breaking changes in release notes
- Provide automated migration tools where possible
- Consider feature flags for gradual migration

## 🔍 Code Review Checklist

### API Design Review
- [ ] Method names follow async/sync conventions
- [ ] Parameters are in correct order (required first, cancellation token last)
- [ ] Generic constraints are meaningful and necessary
- [ ] Return types use ValueTask<T> appropriately
- [ ] Exceptions are specific and meaningful
- [ ] XML documentation is complete and accurate

### Performance Review
- [ ] No unnecessary allocations in hot paths
- [ ] Proper use of Span<T> and Memory<T>
- [ ] Object pooling considered for frequent allocations
- [ ] Async methods don't block synchronously
- [ ] Cancellation tokens are properly propagated

### Testing Review
- [ ] Unit tests cover happy path and edge cases
- [ ] Integration tests validate end-to-end scenarios
- [ ] Performance tests validate targets are met
- [ ] Error handling tests validate exception behavior
- [ ] Async tests properly handle cancellation

---

**Last Updated**: $(date)  
**Version**: 1.0  
**Status**: Draft