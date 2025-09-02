using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

namespace StreamDingo.Benchmarks;

/// <summary>
/// Benchmarks for hash-based integrity verification operations
/// These benchmarks focus on the core hashing functionality that StreamDingo will use
/// </summary>
[Config(typeof(Config))]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[CategoriesColumn]
public class HashBenchmarks
{
    private byte[] _smallData = Array.Empty<byte>();
    private byte[] _mediumData = Array.Empty<byte>();
    private byte[] _largeData = Array.Empty<byte>();
    private readonly SHA256 _sha256 = SHA256.Create();

    [GlobalSetup]
    public void Setup()
    {
        // Create test data of different sizes
        _smallData = Encoding.UTF8.GetBytes(new string('A', 100));       // 100 bytes
        _mediumData = Encoding.UTF8.GetBytes(new string('B', 10000));    // 10KB
        _largeData = Encoding.UTF8.GetBytes(new string('C', 1000000));   // 1MB
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _sha256?.Dispose();
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "SHA256", "Small")]
    public byte[] SHA256_Small()
    {
        // TODO: Replace with alexwiese/hashstamp integration
        // var hashProvider = new HashStampProvider();
        // return hashProvider.CalculateHash(_smallData);

        return _sha256.ComputeHash(_smallData);
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "SHA256", "Medium")]
    public byte[] SHA256_Medium()
    {
        return _sha256.ComputeHash(_mediumData);
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "SHA256", "Large")]
    public byte[] SHA256_Large()
    {
        return _sha256.ComputeHash(_largeData);
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Verification")]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public bool HashVerificationBatch(int verificationCount)
    {
        // TODO: Implement when HashStamp integration is ready
        // var hashProvider = new HashStampProvider();
        // var expectedHash = hashProvider.CalculateHash(typeof(TestEventHandler));

        byte[] expectedHash = _sha256.ComputeHash(_smallData);
        bool allValid = true;

        for (int i = 0; i < verificationCount; i++)
        {
            byte[] currentHash = _sha256.ComputeHash(_smallData);
            allValid = allValid && CompareHashes(expectedHash, currentHash);
        }

        return allValid;
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "EventHandler", "CodeHash")]
    public string EventHandlerCodeHash()
    {
        // TODO: Replace with actual event handler code hashing using alexwiese/hashstamp
        // var hashProvider = new HashStampProvider();
        // return hashProvider.CalculateHash(typeof(TestEventHandler));

        // Placeholder: simulate hashing event handler code
        string typeInfo = typeof(TestEventHandler).ToString();
        byte[] data = Encoding.UTF8.GetBytes(typeInfo);
        byte[] hash = _sha256.ComputeHash(data);
        return Convert.ToBase64String(hash);
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Snapshot", "Integrity")]
    [Arguments(10)]
    [Arguments(100)]
    public Dictionary<string, string> SnapshotIntegrityCheck(int snapshotCount)
    {
        var results = new Dictionary<string, string>();

        for (int i = 0; i < snapshotCount; i++)
        {
            // TODO: Replace with actual snapshot hashing
            // var snapshot = snapshotStore.GetSnapshot($"stream-{i}");
            // var hash = hashProvider.CalculateSnapshotHash(snapshot);

            var mockSnapshot = new { Id = $"stream-{i}", Value = i, Timestamp = DateTime.UtcNow };
            string json = System.Text.Json.JsonSerializer.Serialize(mockSnapshot);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] hash = _sha256.ComputeHash(data);
            results[$"stream-{i}"] = Convert.ToBase64String(hash);
        }

        return results;
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Performance", "Comparison")]
    public void HashAlgorithmComparison()
    {
        byte[] data = _mediumData;

        // Compare different hash algorithms for StreamDingo use cases
        using var md5 = MD5.Create();
        using var sha1 = SHA1.Create();
        using var sha256 = SHA256.Create();

        byte[] md5Hash = md5.ComputeHash(data);
        byte[] sha1Hash = sha1.ComputeHash(data);
        byte[] sha256Hash = sha256.ComputeHash(data);

        // In production, we'll use alexwiese/hashstamp which may use different algorithms
    }

    private static bool CompareHashes(byte[] hash1, byte[] hash2)
    {
        if (hash1.Length != hash2.Length)
        {
            return false;
        }

        for (int i = 0; i < hash1.Length; i++)
        {
            if (hash1[i] != hash2[i])
            {
                return false;
            }
        }

        return true;
    }
}

/// <summary>
/// Placeholder for future event handler implementation
/// </summary>
public class TestEventHandler
{
    public string Apply(object previousState, object eventData)
    {
        // TODO: Implement actual event handler logic
        return $"Processed: {eventData}";
    }
}
