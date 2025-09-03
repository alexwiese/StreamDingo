using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using StreamDingo;

namespace StreamDingo.Benchmarks;

/// <summary>
/// Benchmarks for core event sourcing operations using the StreamDingo library
/// </summary>
[Config(typeof(Config))]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[CategoriesColumn]
public class EventSourcingBenchmarks
{
    private List<IEvent> _events = new();
    private IEventStore _eventStore = null!;
    private ISnapshotStore<CounterSnapshot> _snapshotStore = null!;
    private IHashProvider _hashProvider = null!;
    private EventStreamManager<CounterSnapshot> _manager = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Initialize StreamDingo components
        _eventStore = new InMemoryEventStore();
        _snapshotStore = new InMemorySnapshotStore<CounterSnapshot>();
        _hashProvider = new BasicHashProvider();
        _manager = new EventStreamManager<CounterSnapshot>(_eventStore, _snapshotStore, _hashProvider);
        
        // Register event handlers
        _manager.RegisterHandler(new CounterIncrementHandler());
        _manager.RegisterHandler(new CounterDecrementHandler());

        // Initialize test data
        _events = new List<IEvent>();
        var random = new Random(42); // Use seed for consistent benchmarks
        for (int i = 0; i < 1000; i++)
        {
            var amount = random.Next(1, 100);
            if (i % 2 == 0)
            {
                _events.Add(new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, i, amount));
            }
            else
            {
                _events.Add(new CounterDecrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, i, amount));
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("EventStore", "Append")]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public async Task AppendEvents(int eventCount)
    {
        var streamId = Guid.NewGuid().ToString();
        long expectedVersion = -1;

        for (int i = 0; i < eventCount; i++)
        {
            var @event = _events[i % _events.Count];
            await _eventStore.AppendEventAsync(streamId, @event, expectedVersion);
            expectedVersion++;
        }
    }

    [Benchmark]
    [BenchmarkCategory("EventStore", "Replay")]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public async Task<CounterSnapshot?> ReplayEvents(int eventCount)
    {
        var streamId = Guid.NewGuid().ToString();
        
        // First, populate the stream with events
        long expectedVersion = -1;
        for (int i = 0; i < Math.Min(eventCount, _events.Count); i++)
        {
            await _eventStore.AppendEventAsync(streamId, _events[i], expectedVersion);
            expectedVersion++;
        }

        // Now replay the events to rebuild state
        return await _manager.ReplayAsync(streamId);
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Calculation")]
    public string HashCalculation()
    {
        var testSnapshot = new CounterSnapshot { Value = 42, EventCount = 10 };
        return _hashProvider.CalculateHash(testSnapshot);
    }

    [Benchmark]
    [BenchmarkCategory("Snapshot", "Creation")]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task<List<Snapshot<CounterSnapshot>>> CreateSnapshots(int snapshotCount)
    {
        var snapshots = new List<Snapshot<CounterSnapshot>>();
        
        for (int i = 0; i < snapshotCount; i++)
        {
            var streamId = $"stream-{i}";
            
            // Add some events to the stream first
            await _eventStore.AppendEventAsync(streamId, 
                new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 0, i * 10), -1);
            
            // Create snapshot
            var snapshot = await _manager.CreateSnapshotAsync(streamId, 0);
            if (snapshot != null)
            {
                snapshots.Add(snapshot);
            }
        }
        
        return snapshots;
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Verification")]
    [Arguments(100)]
    [Arguments(1000)]
    public bool HashVerification(int verificationCount)
    {
        var testSnapshot = new CounterSnapshot { Value = 42, EventCount = 10 };
        var expectedHash = _hashProvider.CalculateHash(testSnapshot);
        bool allValid = true;

        for (int i = 0; i < verificationCount; i++)
        {
            allValid = allValid && _hashProvider.VerifyHash(testSnapshot, expectedHash);
        }

        return allValid;
    }

    [Benchmark]
    [BenchmarkCategory("Memory", "Allocation")]
    [Arguments(1000)]
    [Arguments(10000)]
    public void MemoryAllocationTest(int objectCount)
    {
        // Benchmark memory allocation patterns typical in event sourcing
        var events = new List<IEvent>(objectCount);
        var states = new List<CounterSnapshot>(objectCount);

        var random = new Random(42);
        for (int i = 0; i < objectCount; i++)
        {
            var amount = random.Next(1, 100);
            events.Add(new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, i, amount));
            states.Add(new CounterSnapshot { Value = i * amount, EventCount = i + 1 });
        }

        // Simulate processing with event handlers
        var handler = new CounterIncrementHandler();
        CounterSnapshot? currentState = null;
        foreach (var evt in events)
        {
            if (evt is CounterIncrementedEvent incrementEvent)
            {
                currentState = handler.Handle(currentState, incrementEvent);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Concurrency", "ThreadSafety")]
    [Arguments(4)]
    [Arguments(8)]
    public async Task ConcurrentAccess(int threadCount)
    {
        var tasks = new Task[threadCount];
        var baseStreamId = Guid.NewGuid().ToString();

        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i;
            tasks[i] = Task.Run(async () =>
            {
                var streamId = $"{baseStreamId}-thread{threadId}";
                long expectedVersion = -1;
                
                for (int j = 0; j < 100; j++)
                {
                    var @event = new CounterIncrementedEvent(
                        Guid.NewGuid(), 
                        DateTimeOffset.UtcNow, 
                        expectedVersion + 1, 
                        j);
                    
                    await _eventStore.AppendEventAsync(streamId, @event, expectedVersion);
                    expectedVersion++;
                }
            });
        }

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    [BenchmarkCategory("Serialization", "Performance")]
    [Arguments(100)]
    [Arguments(1000)]
    public void SerializationBenchmark(int eventCount)
    {
        var events = new List<IEvent>();
        var random = new Random(42);
        
        for (int i = 0; i < eventCount; i++)
        {
            var amount = random.Next(1, 100);
            events.Add(new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, i, amount));
        }

        // Test serialization performance used by hash provider
        foreach (var evt in events)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(evt);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<CounterIncrementedEvent>(json);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Snapshot", "Validation")]
    [Arguments(10)]
    [Arguments(100)]
    public async Task<bool> ValidateSnapshots(int snapshotCount)
    {
        var snapshots = new List<Snapshot<CounterSnapshot>>();
        
        // Create snapshots with real StreamDingo functionality
        for (int i = 0; i < snapshotCount; i++)
        {
            var streamId = $"validation-stream-{i}";
            await _eventStore.AppendEventAsync(streamId, 
                new CounterIncrementedEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, 0, i * 10), -1);
            
            var snapshot = await _manager.CreateSnapshotAsync(streamId, 0);
            if (snapshot != null)
            {
                snapshots.Add(snapshot);
            }
        }

        // Validate all snapshots
        bool allValid = true;
        foreach (var snapshot in snapshots)
        {
            if (snapshot.Data != null)
            {
                allValid = allValid && _hashProvider.VerifyHash(snapshot.Data, snapshot.Hash);
            }
        }
        
        return allValid;
    }
}

/// <summary>
/// Custom configuration for StreamDingo benchmarks
/// </summary>
public class Config : ManualConfig
{
    public Config()
    {
        AddJob(Job.Default
            .WithId("Default")
            .AsBaseline());

        AddJob(Job.Default
            .WithId("Throughput")
            .WithRuntime(BenchmarkDotNet.Environments.CoreRuntime.Core90));

        // Add additional exporters and diagnosers
        AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
        AddExporter(BenchmarkDotNet.Exporters.HtmlExporter.Default);
        AddExporter(BenchmarkDotNet.Exporters.Json.JsonExporter.Brief);
        AddExporter(BenchmarkDotNet.Exporters.Csv.CsvExporter.Default);

        // Add memory and threading diagnosers
        AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);
        AddDiagnoser(BenchmarkDotNet.Diagnosers.ThreadingDiagnoser.Default);

        // Configure validation
        AddValidator(BenchmarkDotNet.Validators.BaselineValidator.FailOnError);
        AddValidator(BenchmarkDotNet.Validators.JitOptimizationsValidator.FailOnError);

        // Add logger for better output
        AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Default);
    }
}

// Test data structures - using the same models as the unit tests
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
