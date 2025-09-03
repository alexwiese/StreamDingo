using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace StreamDingo.Benchmarks;

/// <summary>
/// Essential benchmarks optimized for CI/PR workflows
/// Focus on core performance metrics with minimal execution time
/// </summary>
[Config(typeof(CiConfig))]
[MemoryDiagnoser(false)]  // Disable for CI speed
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob]
[CategoriesColumn]
public class CiEssentialBenchmarks
{
    private List<object> _events = new();
    private readonly byte[] _testData = System.Text.Encoding.UTF8.GetBytes("TestDataForHashing");

    [GlobalSetup]
    public void Setup()
    {
        // Minimal test data for CI speed
        _events = new List<object>();
        for (int i = 0; i < 100; i++)  // Reduced from 1000
        {
            _events.Add(new TestEvent($"Event {i}", i));
        }
    }

    /// <summary>
    /// Core event appending performance - essential for event sourcing
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Core", "EventStore")]
    [Arguments(100)]  // Single argument for CI speed
    public void AppendEventsCore(int eventCount)
    {
        var list = new List<object>();
        for (int i = 0; i < Math.Min(eventCount, _events.Count); i++)
        {
            list.Add(_events[i]);
        }
    }

    /// <summary>
    /// Event replay performance - core event sourcing operation
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Core", "Replay")]
    [Arguments(100)]
    public void ReplayEventsCore(int eventCount)
    {
        var state = new TestState("Initial", 0);
        for (int i = 0; i < Math.Min(eventCount, _events.Count); i++)
        {
            if (_events[i] is TestEvent evt)
            {
                state = state with { Name = evt.Name, Value = evt.Value };
            }
        }
    }

    /// <summary>
    /// Hash calculation performance - critical for integrity verification
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Core", "Hash")]
    public void HashCalculationCore()
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] hash = sha256.ComputeHash(_testData);
    }

    /// <summary>
    /// Memory allocation patterns typical in event sourcing
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Core", "Memory")]
    [Arguments(100)]  // Reduced for CI
    public void MemoryAllocationCore(int objectCount)
    {
        var events = new List<TestEvent>(objectCount);
        var states = new List<TestState>(objectCount);

        for (int i = 0; i < objectCount; i++)
        {
            events.Add(new TestEvent($"Event {i}", i));
            states.Add(new TestState($"State {i}", i));
        }

        // Simulate processing
        foreach (var evt in events)
        {
            var state = new TestState(evt.Name, evt.Value);
        }
    }

    /// <summary>
    /// Serialization performance - important for event persistence
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Core", "Serialization")]
    [Arguments(50)]  // Reduced for CI
    public void SerializationCore(int eventCount)
    {
        var events = new List<TestEvent>();
        for (int i = 0; i < eventCount; i++)
        {
            events.Add(new TestEvent($"Event {i}", i));
        }

        foreach (var evt in events)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(evt);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<TestEvent>(json);
        }
    }
}

/// <summary>
/// Ultra-minimal benchmarks for when time is extremely constrained
/// Only the most critical performance indicators
/// </summary>
[Config(typeof(UltraFastCiConfig))]
[MemoryDiagnoser(false)]
[SimpleJob]
public class CiMinimalBenchmarks
{
    private readonly List<object> _events = new() { new TestEvent("Test", 1) };
    private readonly byte[] _data = System.Text.Encoding.UTF8.GetBytes("Test");

    [Benchmark]
    [BenchmarkCategory("Minimal", "Append")]
    public void MinimalAppend()
    {
        var list = new List<object> { _events[0] };
    }

    [Benchmark]
    [BenchmarkCategory("Minimal", "Hash")]
    public void MinimalHash()
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        sha.ComputeHash(_data);
    }
}