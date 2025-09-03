---
Layout: _Layout
Title: performance
---
# Performance

StreamDingo is designed for high-performance event sourcing scenarios with careful attention to memory allocations and throughput.

## Performance Targets

| Operation | Target Performance |
|-----------|-------------------|
| Event Append | >100,000 events/second |
| Event Replay | >500,000 events/second |
| Memory Usage | <1GB for 1M events with snapshots |
| Latency (P99) | <1ms for single operations |

## Benchmarks

StreamDingo includes a comprehensive benchmark suite using BenchmarkDotNet. You can run the benchmarks to see performance on your hardware:

```bash
cd benchmarks/StreamDingo.Benchmarks
dotnet run --configuration Release
```

## Performance Tips

### Event Design
- Keep events small and focused
- Use value types where possible
- Avoid deep object hierarchies in events

### Handler Optimization
- Make event handlers pure functions
- Avoid expensive operations in handlers
- Use immutable data structures

### Storage Considerations
- Choose appropriate storage providers for your use case
- Consider using snapshot intervals to balance replay speed vs storage
- Monitor memory usage with large event streams

## Monitoring Performance

### Memory Profiling
Use tools like:
- dotMemory
- PerfView
- Application Insights

### Throughput Monitoring
- Track events per second metrics
- Monitor replay times
- Set up alerts for performance regressions

## Continuous Performance Testing

StreamDingo includes automated performance regression detection in CI/CD. Each pull request automatically:

1. Runs benchmarks on the PR branch
2. Compares against the main branch baseline
3. Reports performance changes in the PR description
4. Flags significant regressions for review

See [Benchmarks](benchmarks.html) for more details on the benchmark infrastructure.
