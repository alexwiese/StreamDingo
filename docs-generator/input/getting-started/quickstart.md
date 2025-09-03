---
Layout: _Layout
Title: quickstart
---
# Quick Start Guide

Get up and running with StreamDingo in minutes!

## Installation

Install StreamDingo via NuGet Package Manager:

```bash
dotnet add package StreamDingo
```

Or via Package Manager Console:

```powershell
Install-Package StreamDingo
```

## Basic Usage

### 1. Define Your Events

```csharp
public record UserCreated(string UserId, string Name, string Email);
public record UserEmailUpdated(string UserId, string NewEmail);
```

### 2. Define Your Aggregate State

```csharp
public record UserAggregate(string Id, string Name, string Email, int Version);
```

### 3. Create Event Handlers

```csharp
public class UserEventHandlers
{
    public static UserAggregate Handle(UserAggregate? state, UserCreated @event)
        => new(@event.UserId, @event.Name, @event.Email, (state?.Version ?? 0) + 1);
    
    public static UserAggregate Handle(UserAggregate state, UserEmailUpdated @event)
        => state with { Email = @event.NewEmail, Version = state.Version + 1 };
}
```

### 4. Set Up Event Store

```csharp
var eventStore = new InMemoryEventStore();
var streamManager = new EventStreamManager(eventStore);
```

### 5. Append Events and Replay

```csharp
// Append events
await streamManager.AppendEventsAsync("user-123", new object[]
{
    new UserCreated("user-123", "John Doe", "john@example.com"),
    new UserEmailUpdated("user-123", "john.doe@example.com")
});

// Replay to get current state
var currentState = await streamManager.ReplayEventsAsync<UserAggregate>("user-123");
Console.WriteLine($"User: {currentState.Name}, Email: {currentState.Email}");
```

## Key Concepts

- **Events**: Immutable facts about what happened in your system
- **Event Handlers**: Pure functions that apply events to aggregate state
- **Snapshots**: Cached aggregate state with hash-based integrity verification
- **Event Replay**: Rebuilding aggregate state by replaying events from snapshots

## Next Steps

- Learn about [Basic Concepts](concepts.html)
- Explore [Event Sourcing Patterns](../guide/event-sourcing.html)
- Check out [Performance Guidelines](../advanced/performance.html)
