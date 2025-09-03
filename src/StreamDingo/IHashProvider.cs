namespace StreamDingo;

/// <summary>
/// Provides hash calculation functionality for integrity verification.
/// </summary>
public interface IHashProvider
{
    /// <summary>
    /// Calculates a hash of the specified object.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>A string representation of the hash.</returns>
    public string CalculateHash(object data);

    /// <summary>
    /// Calculates a hash of the specified event handler's code.
    /// This is used to detect when event handler logic has changed.
    /// </summary>
    /// <param name="eventHandlerType">The type of the event handler.</param>
    /// <returns>A hash of the event handler's method body.</returns>
    public string CalculateHandlerHash(Type eventHandlerType);

    /// <summary>
    /// Verifies that the provided data matches the expected hash.
    /// </summary>
    /// <param name="data">The data to verify.</param>
    /// <param name="expectedHash">The expected hash value.</param>
    /// <returns>True if the hash matches; otherwise, false.</returns>
    public bool VerifyHash(object data, string expectedHash);
}
