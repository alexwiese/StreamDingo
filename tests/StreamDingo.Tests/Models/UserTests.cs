using StreamDingo.Core.Models;

namespace StreamDingo.Tests.Models;

/// <summary>
/// Unit tests for the User model
/// Demonstrates test-driven development patterns that work well with GitHub Copilot
/// </summary>
public class UserTests
{
    [Fact]
    public void User_Should_Initialize_With_Default_Values()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(string.Empty, user.Username);
        Assert.Equal(string.Empty, user.Email);
        Assert.False(user.IsVerified);
        Assert.True(user.CanStream);
        Assert.Equal(0, user.FollowerCount);
        Assert.Equal(0, user.FollowingCount);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
        Assert.Null(user.LastLoginAt);
    }

    [Theory]
    [InlineData("testuser", "test@example.com")]
    [InlineData("streamer123", "streamer@streaming.com")]
    [InlineData("gamer_pro", "gamer@games.net")]
    public void User_Should_Accept_Valid_Username_And_Email(string username, string email)
    {
        // Arrange
        var user = new User();

        // Act
        user.Username = username;
        user.Email = email;

        // Assert
        Assert.Equal(username, user.Username);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void User_Should_Track_Creation_Time()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var user = new User();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(user.CreatedAt >= beforeCreation);
        Assert.True(user.CreatedAt <= afterCreation);
    }

    // TODO: Add more comprehensive tests
    // Copilot can help generate tests for:
    // - Email validation patterns
    // - Username length constraints
    // - Biography length limits
    // - Profile picture URL validation
    // - Follower/following count edge cases
}

/// <summary>
/// Test data builder for User model
/// This pattern works exceptionally well with GitHub Copilot for generating test data
/// </summary>
public class UserTestDataBuilder
{
    private User _user = new();

    public UserTestDataBuilder WithUsername(string username)
    {
        _user.Username = username;
        return this;
    }

    public UserTestDataBuilder WithEmail(string email)
    {
        _user.Email = email;
        return this;
    }

    public UserTestDataBuilder WithDisplayName(string displayName)
    {
        _user.DisplayName = displayName;
        return this;
    }

    public UserTestDataBuilder AsVerified()
    {
        _user.IsVerified = true;
        return this;
    }

    public UserTestDataBuilder WithFollowers(int count)
    {
        _user.FollowerCount = count;
        return this;
    }

    // TODO: Add more builder methods
    // Copilot will suggest additional fluent methods for test data setup

    public User Build() => _user;
}