# Benchmark Performance Optimization

This document describes the benchmark performance optimizations implemented to speed up PR workflow execution.

## Problem

The original PR benchmark workflow was taking too long (5+ minutes), causing delays in the development feedback loop. This was due to:

1. **Comprehensive BenchmarkDotNet Configuration**: Default configuration runs extensive warmup, multiple iterations, and detailed profiling
2. **Duplicate Execution**: Running benchmarks on both PR and main branches sequentially
3. **No Caching**: Building and restoring dependencies from scratch each time
4. **Fixed Configuration**: No differentiation between documentation vs. code changes

## Solution

### CI-Optimized Benchmark Configurations

Created three specialized benchmark configurations for CI environments:

#### 1. Essential Benchmarks (`CiEssentialBenchmarks`)
- **Purpose**: Core performance metrics for regular PR reviews
- **Configuration**: 3 iterations, 1 warmup, minimal profiling
- **Runtime**: ~30-60 seconds
- **Use Case**: Standard code changes

#### 2. Minimal Benchmarks (`CiMinimalBenchmarks`) 
- **Purpose**: Ultra-fast smoke tests for low-risk changes
- **Configuration**: 1 iteration, 0 warmup, no profiling
- **Runtime**: ~10-20 seconds  
- **Use Case**: Documentation changes, minor fixes

#### 3. Ultra-Fast Configuration (`UltraFastCiConfig`)
- **Purpose**: Maximum speed with basic regression detection
- **Configuration**: ColdStart strategy, minimal measurement
- **Runtime**: ~5-15 seconds
- **Use Case**: Emergency fixes, trivial changes

### Workflow Optimizations

1. **Dynamic Mode Selection**: Automatically chooses benchmark mode based on change type
2. **Caching**: Aggressive caching of .NET packages and build artifacts  
3. **Parallel Optimization**: Optimized build sequence and artifact sharing
4. **Timeout Reduction**: Reduced from 300s to 120s timeout for CI configurations
5. **Smart Skip Logic**: Skip benchmarks for documentation-only changes

### Performance Comparison

| Configuration | Original | Essential | Minimal | Improvement |
|--------------|----------|-----------|---------|-------------|
| **Total Time** | 5-8 minutes | 2-3 minutes | 30-60 seconds | 60-90% faster |
| **Accuracy** | High | Good | Basic | Trade-off acceptable |
| **Use Case** | Detailed analysis | PR feedback | Smoke test | Context-specific |

## Usage

### Automatic Mode (Recommended)
The workflow automatically selects the appropriate benchmark mode:
- **Code changes**: Essential benchmarks
- **Documentation only**: Minimal benchmarks  
- **Mixed changes**: Essential benchmarks

### Manual Override
You can override the benchmark mode via workflow dispatch:

```yaml
benchmark_mode: 
  - 'essential'  # Default for code changes
  - 'minimal'    # For quick smoke tests
  - 'fast'       # Runs both essential and minimal
```

### Local Development

For local detailed benchmarking, continue using the full configuration:

```bash
# Full detailed benchmarks (original behavior)
dotnet run --configuration Release

# CI-optimized benchmarks
dotnet run --configuration Release -- --ci-mode essential
dotnet run --configuration Release -- --ci-mode minimal
```

## Implementation Details

### New Files Added

1. **`CiConfig.cs`**: Fast and ultra-fast BenchmarkDotNet configurations
2. **`CiEssentialBenchmarks.cs`**: Core benchmarks optimized for CI speed
3. **Updated `Program.cs`**: Support for CI mode selection

### Workflow Changes

1. **Dynamic mode detection**: Analyzes PR changes to select appropriate benchmark level
2. **Caching strategy**: Caches NuGet packages and build artifacts
3. **Timeout optimization**: Reduced timeouts matching expected execution times
4. **Error handling**: Graceful handling of timeout scenarios

### Benchmark Selection Criteria

The essential benchmarks focus on:
- **Core Operations**: Event appending, replay, hashing
- **Memory Patterns**: Allocation patterns typical in event sourcing
- **Serialization**: Critical for event persistence
- **Single Parameter Sets**: Reduced parameter combinations for speed

## Benefits

1. **Faster Feedback**: 60-90% reduction in benchmark execution time
2. **Reduced CI Load**: Less resource consumption per PR
3. **Flexible Configuration**: Appropriate benchmarks for change type
4. **Maintained Accuracy**: Sufficient precision for performance regression detection
5. **Better Developer Experience**: Quicker PR review cycles

## Future Enhancements

1. **Parallel Execution**: Run PR and main branch benchmarks in parallel
2. **Smart Filtering**: Automatically filter benchmarks based on changed code areas
3. **Progressive Benchmarking**: Run minimal first, then escalate if issues detected
4. **Baseline Caching**: Cache main branch results to avoid repeated execution

## Configuration Reference

### Essential Mode
```csharp
.WithLaunchCount(1)
.WithWarmupCount(1) 
.WithIterationCount(3)
.WithInvocationCount(1024)
```

### Minimal Mode  
```csharp
.WithLaunchCount(1)
.WithWarmupCount(0)
.WithIterationCount(1) 
.WithInvocationCount(256)
```

This optimization maintains the quality of performance monitoring while dramatically improving the speed of the PR review process.