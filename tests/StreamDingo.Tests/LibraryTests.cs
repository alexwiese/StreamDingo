namespace StreamDingo.Tests;

public class LibraryTests
{
    [Fact]
    public void GetLibraryName_ReturnsCorrectName()
    {
        // Act
        string result = Library.GetLibraryName();

        // Assert
        result.Should().Be("StreamDingo");
    }
}
