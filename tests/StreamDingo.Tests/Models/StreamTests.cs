using StreamDingo.Core.Models;

namespace StreamDingo.Tests.Models;

/// <summary>
/// Unit tests for the Stream model
/// These tests demonstrate patterns that GitHub Copilot can help expand and maintain
/// </summary>
public class StreamTests
{
    [Fact]
    public void Stream_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var stream = new StreamDingo.Core.Models.Stream();

        // Assert
        Assert.NotEqual(Guid.Empty, stream.Id);
        Assert.Equal(string.Empty, stream.Title);
        Assert.Equal(string.Empty, stream.StreamerId);
        Assert.Equal(0, stream.ViewerCount);
        Assert.False(stream.IsLive);
        Assert.Equal(StreamQuality.Medium, stream.Quality);
        Assert.True(stream.IsFamilyFriendly);
        Assert.Empty(stream.Tags);
        Assert.Null(stream.StartedAt);
        Assert.Null(stream.EndedAt);
    }

    [Fact]
    public void Stream_Should_Allow_Setting_Properties()
    {
        // Arrange
        var stream = new StreamDingo.Core.Models.Stream();
        var title = "Test Stream";
        var streamerId = "user123";
        var category = "Gaming";

        // Act
        stream.Title = title;
        stream.StreamerId = streamerId;
        stream.Category = category;
        stream.IsLive = true;
        stream.ViewerCount = 100;

        // Assert
        Assert.Equal(title, stream.Title);
        Assert.Equal(streamerId, stream.StreamerId);
        Assert.Equal(category, stream.Category);
        Assert.True(stream.IsLive);
        Assert.Equal(100, stream.ViewerCount);
    }

    [Theory]
    [InlineData(StreamQuality.Low)]
    [InlineData(StreamQuality.Medium)]
    [InlineData(StreamQuality.High)]
    [InlineData(StreamQuality.Ultra)]
    public void Stream_Should_Accept_All_Quality_Levels(StreamQuality quality)
    {
        // Arrange
        var stream = new StreamDingo.Core.Models.Stream();

        // Act
        stream.Quality = quality;

        // Assert
        Assert.Equal(quality, stream.Quality);
    }

    // TODO: Add more test cases
    // Copilot can suggest additional test scenarios:
    // - Test validation scenarios
    // - Test edge cases for viewer counts
    // - Test stream lifecycle (start/end times)
    // - Test tag management
}