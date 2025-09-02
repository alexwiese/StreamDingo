using FluentAssertions;

namespace StreamDingo.Tests;

/// <summary>
/// Tests for the EventBase class
/// </summary>
public class EventBaseTests
{
    [Fact]
    public void Constructor_WithValidHandlerCodeHash_SetsPropertiesCorrectly()
    {
        // Arrange
        var handlerCodeHash = "test-hash-123";
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var testEvent = new TestEvent(handlerCodeHash);

        // Assert
        var afterCreation = DateTimeOffset.UtcNow;
        testEvent.HandlerCodeHash.Should().Be(handlerCodeHash);
        testEvent.EventId.Should().NotBe(Guid.Empty);
        testEvent.Timestamp.Should().BeAfter(beforeCreation).And.BeBefore(afterCreation.AddMilliseconds(100));
    }

    [Fact]
    public void Constructor_WithNullHandlerCodeHash_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var action = () => new TestEvent(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("handlerCodeHash");
    }

    [Fact]
    public void EventId_IsUniqueForEachInstance()
    {
        // Arrange & Act
        var event1 = new TestEvent("hash1");
        var event2 = new TestEvent("hash2");

        // Assert
        event1.EventId.Should().NotBe(event2.EventId);
    }
}

/// <summary>
/// Test event implementation for unit tests
/// </summary>
public class TestEvent : EventBase<TestEntity>
{
    public string TestData { get; }

    public TestEvent(string handlerCodeHash, string testData = "default-data") : base(handlerCodeHash)
    {
        TestData = testData;
    }
}