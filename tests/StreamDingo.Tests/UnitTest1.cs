using StreamDingo.Core;

namespace StreamDingo.Tests;

public class StreamServiceTests
{
    [Fact]
    public void GetStreamStatus_ReturnsActiveStatus()
    {
        var service = new StreamService();
        var result = service.GetStreamStatus();
        Assert.Equal("Stream is active", result);
    }

    [Fact]
    public void GetActiveStreams_ReturnsStreamCount()
    {
        var service = new StreamService();
        var result = service.GetActiveStreams();
        Assert.True(result > 0);
    }
}