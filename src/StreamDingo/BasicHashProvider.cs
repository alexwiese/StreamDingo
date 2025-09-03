using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace StreamDingo;

/// <summary>
/// Basic implementation of <see cref="IHashProvider"/> using SHA-256.
/// For production use, consider integrating with alexwiese/hashstamp for handler code hashing.
/// </summary>
public class BasicHashProvider : IHashProvider
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <inheritdoc />
    public string CalculateHash(object data)
    {
        if (data == null)
        {
            return ComputeHash("null");
        }

        string json = JsonSerializer.Serialize(data, _jsonOptions);
        return ComputeHash(json);
    }

    /// <inheritdoc />
    public string CalculateHandlerHash(Type eventHandlerType)
    {
        ArgumentNullException.ThrowIfNull(eventHandlerType);

        // TODO: Integrate with alexwiese/hashstamp for proper method body hashing
        // For now, use the type's full name and assembly version as a basic hash
        string typeInfo = $"{eventHandlerType.FullName}|{eventHandlerType.Assembly.GetName().Version}";
        return ComputeHash(typeInfo);
    }

    /// <inheritdoc />
    public bool VerifyHash(object data, string expectedHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedHash);
        string actualHash = CalculateHash(data);
        return string.Equals(actualHash, expectedHash, StringComparison.Ordinal);
    }

    /// <summary>
    /// Computes a SHA-256 hash of the specified input.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>The hash as a hexadecimal string.</returns>
    private static string ComputeHash(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
