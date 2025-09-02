using FluentAssertions;

namespace StreamDingo.Tests;

/// <summary>
/// Tests for the Sha256HashProvider class
/// </summary>
public class Sha256HashProviderTests
{
    private readonly Sha256HashProvider _hashProvider = new();

    [Fact]
    public void ComputeHash_WithSameObject_ReturnsSameHash()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", Value = 42 };

        // Act
        var hash1 = _hashProvider.ComputeHash(entity);
        var hash2 = _hashProvider.ComputeHash(entity);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ComputeHash_WithDifferentObjects_ReturnsDifferentHashes()
    {
        // Arrange
        var entity1 = new TestEntity { Name = "Test1", Value = 42 };
        var entity2 = new TestEntity { Name = "Test2", Value = 42 };

        // Act
        var hash1 = _hashProvider.ComputeHash(entity1);
        var hash2 = _hashProvider.ComputeHash(entity2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeHash_WithNullObject_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var action = () => _hashProvider.ComputeHash(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ComputeCodeHash_WithSameCode_ReturnsSameHash()
    {
        // Arrange
        var code = "public void TestMethod() { return; }";

        // Act
        var hash1 = _hashProvider.ComputeCodeHash(code);
        var hash2 = _hashProvider.ComputeCodeHash(code);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ComputeCodeHash_WithDifferentCode_ReturnsDifferentHashes()
    {
        // Arrange
        var code1 = "public void TestMethod1() { return; }";
        var code2 = "public void TestMethod2() { return; }";

        // Act
        var hash1 = _hashProvider.ComputeCodeHash(code1);
        var hash2 = _hashProvider.ComputeCodeHash(code2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeCodeHash_WithNullCode_ThrowsArgumentNullException()
    {
        // Arrange & Act
        var action = () => _hashProvider.ComputeCodeHash(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ComputeHash_ReturnsValidBase64String()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test", Value = 42 };

        // Act
        var hash = _hashProvider.ComputeHash(entity);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        
        // Should be valid base64
        var action = () => Convert.FromBase64String(hash);
        action.Should().NotThrow();
    }
}