using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using StreamDingo;

namespace StreamDingo.Benchmarks;

/// <summary>
/// Benchmarks for hash-based integrity verification operations using StreamDingo's BasicHashProvider
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
    private IHashProvider _hashProvider = null!;
    private CounterSnapshot _testSnapshot = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Initialize StreamDingo hash provider
        _hashProvider = new BasicHashProvider();
        
        // Create test snapshot
        _testSnapshot = new CounterSnapshot { Value = 42, EventCount = 100 };
        
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
    [BenchmarkCategory("Hash", "StreamDingo", "Snapshot")]
    public string StreamDingo_HashSnapshot()
    {
        return _hashProvider.CalculateHash(_testSnapshot);
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "StreamDingo", "EventHandler")]
    public string StreamDingo_HashEventHandler()
    {
        return _hashProvider.CalculateHandlerHash(typeof(CounterIncrementHandler));
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "SHA256", "Small")]
    public byte[] SHA256_Small()
    {
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
    public bool StreamDingo_HashVerificationBatch(int verificationCount)
    {
        var expectedHash = _hashProvider.CalculateHash(_testSnapshot);
        bool allValid = true;

        for (int i = 0; i < verificationCount; i++)
        {
            allValid = allValid && _hashProvider.VerifyHash(_testSnapshot, expectedHash);
        }

        return allValid;
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Legacy")]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(10000)]
    public bool Legacy_HashVerificationBatch(int verificationCount)
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
    [BenchmarkCategory("Hash", "EventHandler", "StreamDingo")]
    public string StreamDingo_EventHandlerCodeHash()
    {
        return _hashProvider.CalculateHandlerHash(typeof(CounterIncrementHandler));
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "EventHandler", "Legacy")]
    public string Legacy_EventHandlerCodeHash()
    {
        // TODO: Replace with actual event handler code hashing using alexwiese/hashstamp
        // var hashProvider = new HashStampProvider();
        // return hashProvider.CalculateHash(typeof(TestEventHandler));

        // Placeholder: simulate hashing event handler code
        string typeInfo = typeof(CounterIncrementHandler).ToString();
        byte[] data = Encoding.UTF8.GetBytes(typeInfo);
        byte[] hash = _sha256.ComputeHash(data);
        return Convert.ToBase64String(hash);
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Snapshot", "StreamDingo")]
    [Arguments(10)]
    [Arguments(100)]
    public Dictionary<string, string> StreamDingo_SnapshotIntegrityCheck(int snapshotCount)
    {
        var results = new Dictionary<string, string>();

        for (int i = 0; i < snapshotCount; i++)
        {
            var snapshot = new CounterSnapshot { Value = i * 10, EventCount = i + 1 };
            string hash = _hashProvider.CalculateHash(snapshot);
            results[$"stream-{i}"] = hash;
        }

        return results;
    }

    [Benchmark]
    [BenchmarkCategory("Hash", "Snapshot", "Legacy")]
    [Arguments(10)]
    [Arguments(100)]
    public Dictionary<string, string> Legacy_SnapshotIntegrityCheck(int snapshotCount)
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


