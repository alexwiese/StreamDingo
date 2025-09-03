using StreamDingo;

namespace StreamDingo.Tests;

/// <summary>
/// Test event for incrementing a counter.
/// </summary>
public record CounterIncrementedEvent(Guid Id, DateTimeOffset Timestamp, long Version, int Amount) : IEvent;

/// <summary>
/// Test event for decrementing a counter.
/// </summary>
public record CounterDecrementedEvent(Guid Id, DateTimeOffset Timestamp, long Version, int Amount) : IEvent;

/// <summary>
/// Test snapshot representing a counter state.
/// </summary>
public class CounterSnapshot
{
    public int Value { get; set; }
    public int EventCount { get; set; }
}

/// <summary>
/// Event handler for counter increment events.
/// </summary>
public class CounterIncrementHandler : IEventHandler<CounterSnapshot, CounterIncrementedEvent>
{
    public CounterSnapshot Handle(CounterSnapshot? previousSnapshot, CounterIncrementedEvent @event)
    {
        var current = previousSnapshot ?? new CounterSnapshot();
        return new CounterSnapshot 
        { 
            Value = current.Value + @event.Amount,
            EventCount = current.EventCount + 1
        };
    }
}

/// <summary>
/// Event handler for counter decrement events.
/// </summary>
public class CounterDecrementHandler : IEventHandler<CounterSnapshot, CounterDecrementedEvent>
{
    public CounterSnapshot Handle(CounterSnapshot? previousSnapshot, CounterDecrementedEvent @event)
    {
        var current = previousSnapshot ?? new CounterSnapshot();
        return new CounterSnapshot 
        { 
            Value = current.Value - @event.Amount,
            EventCount = current.EventCount + 1
        };
    }
}

/// <summary>
/// Comprehensive tests for event sourcing functionality.
/// </summary>
public class EventSourcingTests
{
    [Fact]
    public async Task EventStreamManager_CanAppendEventAndReplay()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();
        var snapshotStore = new InMemorySnapshotStore<CounterSnapshot>();
        var hashProvider = new BasicHashProvider();
        var manager = new EventStreamManager<CounterSnapshot>(eventStore, snapshotStore, hashProvider);
        
        manager.RegisterHandler(new CounterIncrementHandler());
        
        var streamId = "test-counter";
        var @event = new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 0, 5);

        // Act
        var state = await manager.AppendEventAsync(streamId, @event, -1);

        // Assert
        state.Should().NotBeNull();
        state!.Value.Should().Be(5);
        state.EventCount.Should().Be(1);
    }

    [Fact]
    public async Task EventStreamManager_CanHandleMultipleEvents()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();
        var snapshotStore = new InMemorySnapshotStore<CounterSnapshot>();
        var hashProvider = new BasicHashProvider();
        var manager = new EventStreamManager<CounterSnapshot>(eventStore, snapshotStore, hashProvider);
        
        manager.RegisterHandler(new CounterIncrementHandler());
        manager.RegisterHandler(new CounterDecrementHandler());
        
        var streamId = "test-counter";

        // Act
        await manager.AppendEventAsync(streamId, new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 0, 10), -1);
        await manager.AppendEventAsync(streamId, new CounterDecrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 1, 3), 0);
        var finalState = await manager.AppendEventAsync(streamId, new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 2, 2), 1);

        // Assert
        finalState.Should().NotBeNull();
        finalState!.Value.Should().Be(9); // 10 - 3 + 2 = 9
        finalState.EventCount.Should().Be(3);
    }

    [Fact]
    public async Task EventStreamManager_CanCreateAndUseSnapshots()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();
        var snapshotStore = new InMemorySnapshotStore<CounterSnapshot>();
        var hashProvider = new BasicHashProvider();
        var manager = new EventStreamManager<CounterSnapshot>(eventStore, snapshotStore, hashProvider);
        
        manager.RegisterHandler(new CounterIncrementHandler());
        
        var streamId = "test-counter";

        // Create some events
        await manager.AppendEventAsync(streamId, new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 0, 5), -1);
        await manager.AppendEventAsync(streamId, new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 1, 3), 0);

        // Act - Create a snapshot
        var snapshot = await manager.CreateSnapshotAsync(streamId, 1);

        // Assert
        snapshot.Should().NotBeNull();
        snapshot!.Data.Should().NotBeNull();
        snapshot.Data!.Value.Should().Be(8);
        snapshot.Data.EventCount.Should().Be(2);
        snapshot.Hash.Should().NotBeNullOrWhiteSpace();
        snapshot.Version.Should().Be(1);
    }

    [Fact]
    public async Task InMemoryEventStore_ThrowsConcurrencyExceptionOnVersionMismatch()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();
        var @event1 = new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 0, 5);
        var @event2 = new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 1, 3);
        
        var streamId = "test-stream";

        // Act - Add first event
        await eventStore.AppendEventAsync(streamId, @event1, -1);

        // Assert - Try to add second event with wrong expected version
        var act = () => eventStore.AppendEventAsync(streamId, @event2, -1); // Should be 0, not -1
        await act.Should().ThrowAsync<ConcurrencyException>()
            .Where(e => e.StreamId == streamId && e.ExpectedVersion == -1 && e.ActualVersion == 0);
    }

    [Fact]
    public async Task InMemoryEventStore_CanReadEventsFromSpecificVersion()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();
        var streamId = "test-stream";
        
        var @event1 = new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 0, 1);
        var @event2 = new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 1, 2);
        var @event3 = new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 2, 3);

        await eventStore.AppendEventAsync(streamId, @event1, -1);
        await eventStore.AppendEventAsync(streamId, @event2, 0);
        await eventStore.AppendEventAsync(streamId, @event3, 1);

        // Act - Read events from version 1
        var events = new List<IEvent>();
        await foreach (var @event in eventStore.ReadEventsAsync(streamId, fromVersion: 1))
        {
            events.Add(@event);
        }

        // Assert
        events.Should().HaveCount(2);
        events[0].Version.Should().Be(1);
        events[1].Version.Should().Be(2);
    }
}