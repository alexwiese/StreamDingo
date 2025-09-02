# StreamDingo Benchmarks

This directory contains performance benchmarks for the StreamDingo event sourcing library using BenchmarkDotNet.

## Quick Start

### Run All Benchmarks
```bash
# Using the benchmark runner script (recommended)
./scripts/run-benchmarks.sh --all

# Or directly with dotnet
dotnet run --project benchmarks/StreamDingo.Benchmarks/StreamDingo.Benchmarks.csproj --configuration Release
```

### Run Specific Categories
```bash
# Hash-related benchmarks only
./scripts/run-benchmarks.sh --category Hash

# Event store benchmarks only
./scripts/run-benchmarks.sh --category EventStore

# Memory allocation benchmarks only
./scripts/run-benchmarks.sh --category Memory
```

### Quick Testing
```bash
# Dry run (single execution) for quick testing
./scripts/run-benchmarks.sh --dry

# Short run for CI/CD environments
./scripts/run-benchmarks.sh --quick
```

## Benchmark Structure

### EventSourcingBenchmarks
Core event sourcing operations benchmarks:
- **AppendEvents**: Event appending performance
- **ReplayEvents**: Event replay and snapshot generation
- **HashCalculation**: Basic hash calculation performance
- **CreateSnapshots**: Snapshot creation benchmarks
- **HashVerification**: Hash verification performance
- **MemoryAllocationTest**: Memory allocation patterns
- **ConcurrentAccess**: Thread-safety and concurrent access
- **SerializationBenchmark**: Event serialization performance
- **ValidateSnapshots**: Snapshot validation benchmarks

### HashBenchmarks
Hash-based integrity verification benchmarks:
- **SHA256_Small/Medium/Large**: Hash performance for different data sizes
- **HashVerificationBatch**: Bulk hash verification performance
- **EventHandlerCodeHash**: Event handler code hashing (placeholder for alexwiese/hashstamp)
- **SnapshotIntegrityCheck**: Snapshot integrity verification
- **HashAlgorithmComparison**: Performance comparison of different hash algorithms

## Benchmark Categories

Benchmarks are organized using the following categories:
- **EventStore**: Event storage and retrieval operations
- **Append**: Event appending operations
- **Replay**: Event replay operations  
- **Hash**: Hash calculation and verification
- **Verification**: Integrity verification operations
- **Snapshot**: Snapshot creation and validation
- **Creation**: Object creation and allocation
- **Memory**: Memory allocation and garbage collection
- **Concurrency**: Thread-safety and concurrent access
- **Serialization**: Data serialization and deserialization
- **Performance**: General performance tests

## Configuration

The benchmarks use a custom configuration (`Config` class) with:
- **Multiple Jobs**: Default and Throughput job configurations
- **Multiple Exporters**: Markdown (GitHub), HTML, JSON, CSV
- **Memory Diagnostics**: Detailed memory allocation tracking
- **Threading Diagnostics**: Threading performance metrics
- **Validation**: Baseline and JIT optimizations validation

## Benchmark Runner Script

The `scripts/run-benchmarks.sh` script provides:
- **Automated execution** with comprehensive options
- **Result storage** and history tracking
- **Baseline creation** and regression detection
- **Multiple output formats** (Markdown, HTML, JSON, CSV)
- **Performance reporting** with summary generation

### Script Options
```bash
./scripts/run-benchmarks.sh [OPTIONS]

Options:
  -h, --help          Show help message
  -f, --filter FILTER Run benchmarks matching pattern
  -c, --category CAT  Run benchmarks in specific category
  -b, --baseline      Save results as baseline
  -r, --regression    Compare against baseline
  -q, --quick         Run quick benchmarks
  -a, --all           Run all benchmarks (default)
  --dry               Run dry benchmarks (single execution)
  --memory            Include memory diagnostics
  --threading         Include threading diagnostics
  --export FORMAT     Export format: markdown, html, json, csv
```

### Examples
```bash
# Create baseline from all benchmarks
./scripts/run-benchmarks.sh --all --baseline

# Check for performance regressions
./scripts/run-benchmarks.sh --all --regression

# Run only hash benchmarks with memory profiling
./scripts/run-benchmarks.sh --category Hash --memory

# Quick test of append operations
./scripts/run-benchmarks.sh --filter "*Append*" --quick
```

## CI/CD Integration

The benchmarks are integrated with GitHub Actions via `.github/workflows/pr-benchmark.yml`:
- **Automatic execution** on pull requests
- **Performance comparison** between main branch and PR
- **Results reporting** in PR descriptions  
- **Regression detection** with automated comments
- **Artifact storage** for benchmark results

## Result Storage

Benchmark results are stored in:
- `benchmark-results/` - JSON results with timestamps
- `BenchmarkDotNet.Artifacts/` - Full BenchmarkDotNet artifacts
- `BenchmarkDotNet.Artifacts/results/` - Detailed reports (HTML, Markdown, CSV)

## Current Status

**Note**: These are placeholder benchmarks as the core StreamDingo library is still under development. The benchmarks currently:
- Use mock implementations for event sourcing operations
- Simulate hash calculations (will integrate with alexwiese/hashstamp)
- Provide realistic performance baselines for future real implementations
- Establish the infrastructure for continuous performance tracking

As the library develops, these placeholder implementations will be replaced with actual StreamDingo operations while maintaining the same benchmark structure and automation.

## Performance Targets

Based on the detailed implementation plan, StreamDingo targets:
- **Event Append**: >100,000 events/second
- **Event Replay**: >500,000 events/second  
- **Memory Usage**: <1GB for 1M events with snapshots
- **Latency**: <1ms P99 for single event operations

The benchmarks will track progress toward these targets and detect regressions during development.