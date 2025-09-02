namespace StreamDingo;

/// <summary>
/// Provides hashing functionality for entities and snapshots
/// </summary>
public interface IHashProvider
{
    /// <summary>
    /// Computes a hash for the given object
    /// </summary>
    /// <param name="obj">The object to hash</param>
    /// <returns>A string representation of the hash</returns>
    string ComputeHash(object obj);

    /// <summary>
    /// Computes a hash for the given code (method, class, etc.)
    /// </summary>
    /// <param name="code">The code to hash</param>
    /// <returns>A string representation of the hash</returns>
    string ComputeCodeHash(string code);
}

/// <summary>
/// Default implementation of IHashProvider using SHA256
/// </summary>
public class Sha256HashProvider : IHashProvider
{
    /// <inheritdoc />
    public string ComputeHash(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        
        var json = System.Text.Json.JsonSerializer.Serialize(obj);
        return ComputeCodeHash(json);
    }

    /// <inheritdoc />
    public string ComputeCodeHash(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        
        var bytes = System.Text.Encoding.UTF8.GetBytes(code);
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}