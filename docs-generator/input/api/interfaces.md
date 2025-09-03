---
Layout: _Layout
Title: interfaces
---
# Core Interfaces

StreamDingo's core interfaces provide the foundation for event sourcing functionality.

!!! info "Auto-Generated Documentation"
    Detailed API documentation is automatically generated from XML comments in the source code. 
    See the [Generated API Reference](generated/index.html) for complete documentation of all public types and members.

## Core Interface Overview

### IEvent
Base interface for all events in the system. Events are immutable data structures that represent something that happened in the domain.

### IEventHandler<TState, TEvent>
Defines event handlers that transform aggregate state. Event handlers are pure functions that take the previous state and an event, returning the new state.

### IEventStore
Manages event persistence and retrieval. The event store is responsible for appending events to streams and reading events for replay.

### ISnapshotStore  
Handles snapshot storage and retrieval. Snapshots are periodic captures of aggregate state used to optimize event replay performance.

### IHashProvider
Provides hash-based integrity verification. The hash provider ensures event handler code changes are detected and handles snapshot integrity verification.

## Design Principles

- **Immutability**: All events and snapshots are immutable
- **Pure Functions**: Event handlers are pure functions with no side effects
- **Hash Verification**: All components support hash-based integrity checking
- **Async-First**: All I/O operations are asynchronous by default
- **Type Safety**: Strong typing throughout with generic support
