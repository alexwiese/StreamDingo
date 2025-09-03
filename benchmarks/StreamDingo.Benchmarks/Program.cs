using BenchmarkDotNet.Running;
using System.Reflection;

namespace StreamDingo.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        // Check for CI-specific benchmark modes
        if (args.Length > 0 && args.Contains("--ci-mode"))
        {
            var ciModeIndex = Array.IndexOf(args, "--ci-mode");
            var mode = ciModeIndex + 1 < args.Length ? args[ciModeIndex + 1] : "essential";
            
            // Remove CI mode args before passing to BenchmarkDotNet
            args = args.Where((_, index) => index != ciModeIndex && index != ciModeIndex + 1).ToArray();
            
            RunCiBenchmarks(mode, args);
        }
        else
        {
            // Default behavior - run all benchmarks
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
    
    private static void RunCiBenchmarks(string mode, string[] args)
    {
        Console.WriteLine($"Running in CI mode: {mode}");
        
        var benchmarkTypes = mode.ToLower() switch
        {
            "essential" => new[] { typeof(CiEssentialBenchmarks) },
            "minimal" => new[] { typeof(CiMinimalBenchmarks) },
            "fast" => new[] { typeof(CiEssentialBenchmarks), typeof(CiMinimalBenchmarks) },
            _ => new[] { typeof(CiEssentialBenchmarks) }
        };
        
        foreach (var benchmarkType in benchmarkTypes)
        {
            Console.WriteLine($"Running benchmarks for: {benchmarkType.Name}");
            BenchmarkRunner.Run(benchmarkType, args: args);
        }
    }
}
