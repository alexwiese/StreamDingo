using StreamDingo;

namespace StreamDingo.Tests;

public class CoreLibraryTests
{
    [Fact]
    public void InMemoryEventStore_CanBeInstantiated()
    {
        // Act
        var eventStore = new InMemoryEventStore();

        // Assert
        eventStore.Should().NotBeNull();
    }

    [Fact]
    public void BasicHashProvider_CanCalculateHash()
    {
        // Arrange
        var hashProvider = new BasicHashProvider();
        var testData = new { Name = "Test", Value = 42 };

        // Act
        var hash = hashProvider.CalculateHash(testData);

        // Assert
        hash.Should().NotBeNullOrWhiteSpace();
        hash.Length.Should().Be(64); // SHA-256 produces 64 hex characters
    }

    [Fact] 
    public async Task InMemoryEventStore_StreamExistsAsync_ReturnsFalseForNonExistentStream()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();

        // Act
        var exists = await eventStore.StreamExistsAsync("non-existent-stream");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task InMemoryEventStore_GetStreamVersionAsync_ReturnsMinusOneForNonExistentStream()
    {
        // Arrange
        var eventStore = new InMemoryEventStore();

        // Act
        var version = await eventStore.GetStreamVersionAsync("non-existent-stream");

        // Assert
        version.Should().Be(-1);
    }
}
