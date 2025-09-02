# StreamDingo

[![CI](https://github.com/alexwiese/StreamDingo/actions/workflows/ci.yml/badge.svg)](https://github.com/alexwiese/StreamDingo/actions/workflows/ci.yml)
[![Release](https://github.com/alexwiese/StreamDingo/actions/workflows/release.yml/badge.svg)](https://github.com/alexwiese/StreamDingo/actions/workflows/release.yml)
[![NuGet Version](https://img.shields.io/nuget/v/StreamDingo)](https://www.nuget.org/packages/StreamDingo/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET library for event sourcing with hash-based integrity verification.

## Features

- **Event Sourcing**: Store and replay events to reconstruct entity state
- **Hash-Based Integrity**: Verify the integrity of events, snapshots, and event handler code
- **Snapshot Management**: Efficient state reconstruction with key snapshots
- **Event Replay**: Handle code changes and event reordering with automatic replay
- **Strongly Typed**: Full C# type safety with generic interfaces
- **Async/Await Support**: Modern asynchronous programming patterns

## Core Concepts

### Events
Events represent state changes in your system. Each event:
- Has a unique identifier and timestamp
- Contains a hash of the event handler code
- Is immutable once created
- Can be applied to entity snapshots

### Snapshots
Snapshots represent the state of an entity at a specific point in time:
- Contains the full entity data
- Has a hash for integrity verification
- Can be marked as "key snapshots" for efficient replay
- Tracks version numbers for ordering

### Event Handlers
Event handlers apply events to snapshots to produce new snapshots:
- Each handler has a code hash for integrity verification
- Handlers are pure functions (snapshot + event → new snapshot)
- Support for automatic replay when handler code changes

### Event Streams
Event streams manage the lifecycle of events and snapshots:
- Append events in order
- Maintain current entity state
- Support replay from any snapshot
- Handle integrity verification

## Installation

```bash
dotnet add package StreamDingo
```

## Quick Start

```csharp
using StreamDingo;

// Define your entity
public class BankAccount
{
    public string AccountId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Owner { get; set; } = string.Empty;
}

// Define events
public class AccountOpenedEvent : EventBase<BankAccount>
{
    public string AccountId { get; }
    public string Owner { get; }
    public decimal InitialDeposit { get; }

    public AccountOpenedEvent(string handlerCodeHash, string accountId, string owner, decimal initialDeposit) 
        : base(handlerCodeHash)
    {
        AccountId = accountId;
        Owner = owner;
        InitialDeposit = initialDeposit;
    }
}

public class MoneyDepositedEvent : EventBase<BankAccount>
{
    public decimal Amount { get; }

    public MoneyDepositedEvent(string handlerCodeHash, decimal amount) : base(handlerCodeHash)
    {
        Amount = amount;
    }
}

// Define event handlers
public class AccountOpenedEventHandler : IEventHandler<BankAccount, AccountOpenedEvent>
{
    private readonly IHashProvider _hashProvider = new Sha256HashProvider();

    public string CodeHash => _hashProvider.ComputeCodeHash("AccountOpenedEventHandler_v1.0");

    public ISnapshot<BankAccount> Apply(ISnapshot<BankAccount> previousSnapshot, AccountOpenedEvent @event)
    {
        var account = new BankAccount
        {
            AccountId = @event.AccountId,
            Owner = @event.Owner,
            Balance = @event.InitialDeposit
        };

        return new Snapshot<BankAccount>(
            @event.AccountId,
            previousSnapshot.Version + 1,
            _hashProvider.ComputeHash(account),
            account
        );
    }
}

// Usage
var hashProvider = new Sha256HashProvider();
var handler = new AccountOpenedEventHandler();

var openEvent = new AccountOpenedEvent(
    handler.CodeHash,
    "ACC-001",
    "John Doe",
    1000m
);

// Apply event to create first snapshot
var initialSnapshot = new Snapshot<BankAccount>("ACC-001", 0, "", new BankAccount(), true);
var newSnapshot = handler.Apply(initialSnapshot, openEvent);

Console.WriteLine($"Account {newSnapshot.Data.AccountId} opened for {newSnapshot.Data.Owner} with balance ${newSnapshot.Data.Balance}");
```

## Documentation

### Interfaces

- **`IEvent<TEntity>`**: Base interface for all events
- **`ISnapshot<TEntity>`**: Represents entity state at a point in time  
- **`IEventHandler<TEntity, TEvent>`**: Handles applying events to snapshots
- **`IEventStream<TEntity>`**: Manages event streams and snapshots
- **`IHashProvider`**: Provides hashing functionality for integrity verification

### Base Classes

- **`EventBase<TEntity>`**: Base implementation for events with common properties
- **`Snapshot<TEntity>`**: Default snapshot implementation
- **`Sha256HashProvider`**: SHA-256 based hash provider

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Roadmap

- [ ] Event store implementations (SQL Server, PostgreSQL, MongoDB)
- [ ] Event stream implementations with persistence
- [ ] Performance optimizations for large event streams
- [ ] Integration with popular DI containers
- [ ] Event versioning and migration utilities
- [ ] Distributed event sourcing patterns
- [ ] Integration with alexwiese/hashstamp library