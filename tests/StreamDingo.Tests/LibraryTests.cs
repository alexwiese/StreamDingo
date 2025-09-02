namespace StreamDingo.Tests;

public class LibraryTests
{
    [Fact]
    public void GetLibraryName_ReturnsCorrectName()
    {
        // Act
        var result = Library.GetLibraryName();

        // Assert
        result.Should().Be("StreamDingo");
    }
}
