<p align="center">
  <img src="assets/logo.svg" alt="StreamDingo Logo" width="400">
</p>

# StreamDingo

<p align="center">
  A high-performance .NET library for event sourcing with hash-based integrity verification and snapshot-based replay
</p>

<p align="center">
  <a href="https://github.com/alexwiese/StreamDingo/actions/workflows/ci.yml">
    <img src="https://github.com/alexwiese/StreamDingo/actions/workflows/ci.yml/badge.svg" alt="Build Status">
  </a>
  <a href="https://codecov.io/gh/alexwiese/StreamDingo">
    <img src="https://codecov.io/gh/alexwiese/StreamDingo/branch/main/graph/badge.svg" alt="Code Coverage">
  </a>
  <a href="https://github.com/alexwiese/StreamDingo/security/advisories">
    <img src="https://img.shields.io/github/security-advisories/gh/alexwiese/StreamDingo" alt="Security Advisories">
  </a>
  <a href="https://dotnet.microsoft.com/download/dotnet">
    <img src="https://img.shields.io/badge/.NET-9.0%2B-512BD4?logo=.net" alt=".NET Version">
  </a>
  <a href="LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="License">
  </a>
  <!-- NuGet Package - uncomment when package is published
  <a href="https://www.nuget.org/packages/StreamDingo">
    <img src="https://img.shields.io/nuget/v/StreamDingo.svg" alt="NuGet Version">
  </a>
  <a href="https://www.nuget.org/packages/StreamDingo">
    <img src="https://img.shields.io/nuget/dt/StreamDingo.svg" alt="NuGet Downloads">
  </a>
  -->
</p></p>

## ✨ Features

- **Event Sourcing Made Simple**: Clean, intuitive API for implementing event sourcing patterns
- **Hash-Based Integrity**: Built-in integrity verification using cryptographic hashes for events, handlers, and snapshots
- **Snapshot-Based Replay**: Efficient event replay from snapshots with intelligent invalidation
- **Code Change Detection**: Automatic detection of event handler changes with smart replay strategies
- **High Performance**: Optimized for throughput with minimal memory allocation
- **Type Safety**: Strongly typed event handlers and snapshots with full generic support
- **Framework Integration**: Seamless integration with ASP.NET Core, dependency injection, and logging

## 🚀 Quick Start

### Installation

Install StreamDingo via NuGet Package Manager:

```bash
dotnet add package StreamDingo
```

Or via Package Manager Console:

```powershell
Install-Package StreamDingo
```

### Basic Usage

```csharp
using StreamDingo;

// Define your events
public record UserCreated(string Name, string Email);
public record UserEmailUpdated(string NewEmail);

// Define your aggregate state
public record UserState(string Name, string Email, bool IsActive = true);

// Create event handlers
public class UserEventHandlers
{
    public static UserState Apply(UserState? state, UserCreated @event)
    {
        return new UserState(@event.Name, @event.Email);
    }

    public static UserState Apply(UserState state, UserEmailUpdated @event)
    {
        return state with { Email = @event.NewEmail };
    }
}

// Use the event store
var eventStore = new StreamDingoEventStore();

// Create a new stream
var streamId = Guid.NewGuid();
await eventStore.AppendAsync(streamId, new UserCreated("John Doe", "john@example.com"));
await eventStore.AppendAsync(streamId, new UserEmailUpdated("john.doe@example.com"));

// Replay events to get current state
var currentState = await eventStore.ReplayAsync<UserState>(streamId);
Console.WriteLine($"User: {currentState.Name}, Email: {currentState.Email}");
```

## 🏗️ Core Concepts

### Event Handlers

Event handlers are pure functions that take a previous state snapshot and an event, returning a new state:

```csharp
public static TState Apply(TState previousState, TEvent @event)
{
    // Apply event logic
    return newState;
}
```

Each event handler is automatically hashed using the [HashStamp](https://github.com/alexwiese/hashstamp) library to detect code changes.

### Snapshots

Snapshots represent the state of an aggregate at a specific point in time:

- Each snapshot is cryptographically hashed for integrity verification
- Snapshots serve as checkpoints for efficient event replay
- The last snapshot represents the current entity state

### Hash-Based Integrity

StreamDingo ensures data integrity through multiple hash layers:

- **Event Handler Hashes**: Detect when business logic changes
- **Snapshot Hashes**: Verify snapshot integrity and detect corruption  
- **Event Hashes**: Ensure event data hasn't been tampered with

### Smart Replay Strategies

When changes are detected, StreamDingo intelligently replays events:

- **Event Order Changes**: Replay from the last stable snapshot
- **Handler Code Changes**: Replay affected event types until snapshot hashes stabilize
- **Corruption Detection**: Validate integrity at each step during replay

## 📖 API Reference

### StreamDingoEventStore

The main entry point for event sourcing operations.

#### Methods

```csharp
// Append events to a stream
Task AppendAsync<T>(Guid streamId, T @event, CancellationToken cancellationToken = default);

// Replay events to rebuild state
Task<TState> ReplayAsync<TState>(Guid streamId, CancellationToken cancellationToken = default);

// Get stream information
Task<StreamInfo> GetStreamInfoAsync(Guid streamId, CancellationToken cancellationToken = default);

// Create snapshots manually
Task CreateSnapshotAsync<TState>(Guid streamId, TState state, CancellationToken cancellationToken = default);
```

### Configuration

Configure StreamDingo in your application:

```csharp
// Program.cs
builder.Services.AddStreamDingo(options =>
{
    options.UseInMemoryStorage(); // or UseSqlServer, UsePostgreSQL, etc.
    options.EnableAutomaticSnapshots(every: 100); // Create snapshots every 100 events
    options.EnableIntegrityChecking(true);
    options.ConfigureHashing(hash => hash.IncludeMethodBodies = true);
});
```

## 🔧 Development

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- Optional: [Git](https://git-scm.com/) for source control

### Building

Clone the repository and build:

```bash
git clone https://github.com/alexwiese/StreamDingo.git
cd StreamDingo
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Code Formatting

We use EditorConfig for consistent code formatting:

```bash
dotnet format
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Quick Contribution Steps

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes and add tests
4. Ensure all tests pass: `dotnet test`
5. Format your code: `dotnet format`
6. Commit your changes: `git commit -m 'Add amazing feature'`
7. Push to your fork: `git push origin feature/amazing-feature`
8. Open a Pull Request

## 📋 Roadmap

- [ ] Additional storage providers (Redis, MongoDB, Event Store DB)
- [ ] GraphQL integration
- [ ] Event versioning and migration tools
- [ ] Performance benchmarking suite
- [ ] Event sourcing best practices documentation
- [ ] Integration with popular CQRS frameworks

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [HashStamp](https://github.com/alexwiese/hashstamp) for code hashing capabilities
- Inspired by event sourcing patterns from the DDD community
- Thanks to all [contributors](https://github.com/alexwiese/StreamDingo/contributors)

## 📞 Support

- 📖 [Documentation](https://github.com/alexwiese/StreamDingo/wiki)
- 🐛 [Bug Reports](https://github.com/alexwiese/StreamDingo/issues)
- 💬 [Discussions](https://github.com/alexwiese/StreamDingo/discussions)
- 📧 [Contact the maintainer](mailto:alex@alexwiese.com)