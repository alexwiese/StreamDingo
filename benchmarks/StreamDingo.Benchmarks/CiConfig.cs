using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Engines;

namespace StreamDingo.Benchmarks;

/// <summary>
/// Ultra-fast configuration optimized for CI/PR benchmarks
/// Prioritizes speed over precision while maintaining relative comparison accuracy
/// </summary>
public class CiConfig : ManualConfig
{
    public CiConfig()
    {
        // Use single ultra-fast job optimized for CI
        AddJob(Job.Default
            .WithId("CiFast")
            .WithStrategy(RunStrategy.Monitoring)  // Skip pilot phase
            .WithLaunchCount(1)                    // Single process launch
            .WithWarmupCount(1)                    // Minimal warmup
            .WithIterationCount(3)                 // Just 3 measurement iterations
            .WithInvocationCount(1024)             // Fixed invocation count
            .WithUnrollFactor(1)                   // No unrolling
            .WithRuntime(CoreRuntime.Core90)
            .WithToolchain(InProcessEmitToolchain.Instance)  // Fastest toolchain
            .AsBaseline());

        // Only essential exporters for CI speed
        AddExporter(BenchmarkDotNet.Exporters.Json.JsonExporter.Brief);
        
        // Minimal diagnosers for speed
        AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);
        
        // Reduced logging for speed
        AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Unicode);
        
        // Configure for CI environment
        AddColumnProvider(BenchmarkDotNet.Columns.DefaultColumnProviders.Instance);
        
        // Stop at first failure to save time
        WithOptions(ConfigOptions.StopOnFirstError);
    }
}

/// <summary>
/// Even faster configuration for ultra-quick CI runs when time is critical
/// Trades accuracy for speed - use when just checking for major regressions
/// </summary>
public class UltraFastCiConfig : ManualConfig
{
    public UltraFastCiConfig()
    {
        // Minimal job configuration
        AddJob(Job.Default
            .WithId("UltraFast")
            .WithStrategy(RunStrategy.ColdStart)   // Skip warmup entirely
            .WithLaunchCount(1)
            .WithWarmupCount(0)                    // No warmup
            .WithIterationCount(1)                 // Single measurement
            .WithInvocationCount(256)              // Small fixed count
            .WithUnrollFactor(1)
            .WithRuntime(CoreRuntime.Core90)
            .WithToolchain(InProcessEmitToolchain.Instance)
            .AsBaseline());

        // Minimal exports
        AddExporter(BenchmarkDotNet.Exporters.Json.JsonExporter.Brief);
        
        // No additional diagnosers for maximum speed
        // AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default); // Commented out for speed
        
        // Minimal logging
        AddLogger(BenchmarkDotNet.Loggers.NullLogger.Instance);
        
        // Stop on first error
        WithOptions(ConfigOptions.StopOnFirstError);
        WithOptions(ConfigOptions.DisableLogFile);
    }
}