namespace StreamDingo.Tests;

public class EventSourcingTests
{
    public class TestSnapshot : ISnapshot
    {
        public string Hash { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string Data { get; init; } = string.Empty;
    }

    public class TestEvent : IEvent
    {
        public string EventId { get; init; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public string Value { get; init; } = string.Empty;
    }

    public class TestEventHandler : IEventHandler<TestSnapshot, TestEvent>
    {
        public string HandlerHash { get; } = "test-handler-v1";

        public TestSnapshot Apply(TestSnapshot previousSnapshot, TestEvent @event)
        {
            return new TestSnapshot
            {
                Hash = $"hash-{@event.EventId}",
                CreatedAt = DateTime.UtcNow,
                Data = previousSnapshot.Data + @event.Value
            };
        }
    }

    [Fact]
    public void BasicEventHandlerTest()
    {
        // Arrange
        var initialSnapshot = new TestSnapshot { Data = "initial" };
        var testEvent = new TestEvent { Value = "-appended" };
        var handler = new TestEventHandler();

        // Act
        var resultSnapshot = handler.Apply(initialSnapshot, testEvent);

        // Assert
        Assert.NotNull(resultSnapshot);
        Assert.NotEqual(initialSnapshot.Hash, resultSnapshot.Hash);
        Assert.Equal("initial-appended", resultSnapshot.Data);
        Assert.Equal("test-handler-v1", handler.HandlerHash);
    }

    [Fact]
    public void EventHandlerHashIntegrityTest()
    {
        // Arrange
        var handler1 = new TestEventHandler();
        var handler2 = new TestEventHandler();

        // Act & Assert
        Assert.Equal(handler1.HandlerHash, handler2.HandlerHash);
    }
}