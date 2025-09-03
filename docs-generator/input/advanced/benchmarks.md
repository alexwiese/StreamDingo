---
Layout: _Layout
Title: benchmarks
---
# Benchmarks

StreamDingo uses [BenchmarkDotNet](https://benchmarkdotnet.org/) to measure and track performance characteristics across different scenarios.

## Running Benchmarks

### Local Development

To run benchmarks locally:

```bash
cd benchmarks/StreamDingo.Benchmarks
dotnet run --configuration Release
```

For specific benchmarks:

```bash
dotnet run --configuration Release -- --filter "*EventAppend*"
```

### Benchmark Categories

The benchmark suite covers:

- **Event Appending**: Throughput of adding events to streams
- **Event Replay**: Speed of reconstructing state from events
- **Hash Calculation**: Performance of integrity verification
- **Snapshot Creation**: Memory and time costs of snapshots
- **Memory Allocation**: GC pressure and memory usage patterns

## Automated Performance Tracking

### PR Benchmarks

Every pull request automatically:

1. **Runs baseline benchmarks** on the main branch
2. **Runs PR benchmarks** on the proposed changes
3. **Compares results** and calculates performance deltas
4. **Updates PR description** with collapsible performance results
5. **Comments on significant changes** (>10% regression/improvement)

### Benchmark Reports

Performance reports include:

```markdown
## 📊 Performance Benchmark Results

| Benchmark | Main | PR | Change | Memory Main | Memory PR | Memory Change |
|-----------|------|----|---------|-----------|---------|-----------   |
| AppendEvents(100) | 124.3 μs | 118.2 μs | 🟢 -4.9% | 2.1 KB | 2.0 KB | 🟢 -4.8% |
| ReplayEvents(1000) | 1.2 ms | 1.3 ms | 🔴 +8.3% | 15.2 KB | 16.1 KB | 🟡 +5.9% |
```

Legend:
- 🟢 = Performance improvement
- 🟡 = Minor change (<10%)
- 🔴 = Performance regression

### Historical Tracking

Benchmark results are:
- Stored as artifacts for 30 days
- Tracked in GitHub Issues for significant changes
- Used to build performance trend reports

## Benchmark Infrastructure

### BenchmarkDotNet Configuration

```csharp
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class EventSourcingBenchmarks
{
    [Benchmark]
    [Arguments(100, 1000, 10000)]
    public void AppendEvents(int eventCount) { /* ... */ }
}
```

### Continuous Integration

The benchmark workflow:

1. **Triggers**: On every PR to main branch
2. **Environment**: Ubuntu Latest with .NET 9.0
3. **Execution**: Short job profile for CI speed
4. **Output**: JSON format for automated parsing
5. **Analysis**: Python script for comparison and reporting

### Skipping Benchmarks

To skip benchmarks on a PR (for documentation-only changes):

```
Add the label: skip-benchmark
```

## Interpreting Results

### Understanding Metrics

- **Mean**: Average execution time
- **Median**: 50th percentile execution time
- **StdDev**: Standard deviation (consistency indicator)
- **Allocated**: Memory allocated per operation

### Performance Thresholds

- **Green (Improvement)**: >5% faster or less memory
- **Yellow (Neutral)**: ±5% change
- **Red (Regression)**: >5% slower or more memory

### When to Be Concerned

Review carefully if:
- Core operations show >10% regression
- Memory usage increases significantly
- New allocations appear in hot paths
- Standard deviation increases (less consistent performance)

## Contributing to Benchmarks

When adding new features:

1. **Add corresponding benchmarks** for new operations
2. **Ensure benchmarks are realistic** and representative
3. **Include both best-case and worst-case scenarios**
4. **Test with different data sizes** using `[Arguments]`
5. **Verify benchmarks pass** before submitting PR

### Benchmark Best Practices

```csharp
[GlobalSetup]
public void Setup()
{
    // Initialize test data once
    // Avoid setup work in benchmark methods
}

[Benchmark]
public void MyBenchmark()
{
    // Keep benchmark focused
    // Avoid external dependencies
    // Use realistic data sizes
}
```
