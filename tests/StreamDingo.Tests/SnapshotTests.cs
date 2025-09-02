using FluentAssertions;

namespace StreamDingo.Tests;

/// <summary>
/// Tests for the Snapshot class
/// </summary>
public class SnapshotTests
{
    [Fact]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
    {
        // Arrange
        var entityId = "test-entity-123";
        var version = 42L;
        var hash = "test-hash";
        var data = new TestEntity { Name = "Test", Value = 123 };
        var isKeySnapshot = true;

        // Act
        var snapshot = new Snapshot<TestEntity>(entityId, version, hash, data, isKeySnapshot);

        // Assert
        snapshot.EntityId.Should().Be(entityId);
        snapshot.Version.Should().Be(version);
        snapshot.Hash.Should().Be(hash);
        snapshot.Data.Should().Be(data);
        snapshot.IsKeySnapshot.Should().Be(isKeySnapshot);
        snapshot.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithNullEntityId_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var action = () => new Snapshot<TestEntity>(null!, 1, "hash", new TestEntity(), false);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("entityId");
    }

    [Fact]
    public void Constructor_WithNullHash_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var action = () => new Snapshot<TestEntity>("entity", 1, null!, new TestEntity(), false);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("hash");
    }

    [Fact]
    public void Constructor_WithNullData_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var action = () => new Snapshot<TestEntity>("entity", 1, "hash", null!, false);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("data");
    }

    [Fact]
    public void Constructor_WithDefaultIsKeySnapshot_SetsFalse()
    {
        // Arrange & Act
        var snapshot = new Snapshot<TestEntity>("entity", 1, "hash", new TestEntity());

        // Assert
        snapshot.IsKeySnapshot.Should().BeFalse();
    }
}

/// <summary>
/// Test entity class for unit tests
/// </summary>
public class TestEntity
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}