# Core Interfaces

StreamDingo's core interfaces provide the foundation for event sourcing functionality.

## IEvent
Base interface for all events in the system.

## IEventHandler<TState, TEvent>
Defines event handlers that transform aggregate state.

## IEventStore
Manages event persistence and retrieval.

## ISnapshotStore  
Handles snapshot storage and retrieval.

## IHashProvider
Provides hash-based integrity verification.

More detailed API documentation will be generated automatically from XML comments in the source code.