# Developer Setup Guide for GitHub Copilot

This guide will help you set up your development environment to get the most out of GitHub Copilot with the StreamDingo project.

## Prerequisites

### 1. GitHub Copilot Subscription
- Individual or Business subscription to GitHub Copilot
- Access through GitHub account settings

### 2. Supported IDE with Copilot Extension

#### Visual Studio Code (Recommended)
```bash
# Install VS Code extensions
code --install-extension GitHub.copilot
code --install-extension GitHub.copilot-chat
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.csdevkit
```

#### Visual Studio 2022
- Install GitHub Copilot extension from Visual Studio Marketplace
- Ensure you have the latest version (17.8+)

#### JetBrains Rider
```bash
# Install Rider plugin
# File -> Settings -> Plugins -> Marketplace -> Search "GitHub Copilot"
```

### 3. .NET Development Environment
```bash
# Install .NET 8 SDK
# Windows (using winget)
winget install Microsoft.DotNet.SDK.8

# macOS (using Homebrew)
brew install --cask dotnet-sdk

# Linux (Ubuntu/Debian)
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

## Project Setup

### 1. Clone and Configure
```bash
git clone https://github.com/alexwiese/StreamDingo.git
cd StreamDingo

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests to verify setup
dotnet test
```

### 2. Configure Copilot Settings

#### VS Code Settings (`.vscode/settings.json`)
```json
{
  "github.copilot.enable": {
    "*": true,
    "yaml": true,
    "plaintext": true,
    "markdown": true,
    "csharp": true
  },
  "github.copilot.advanced": {
    "length": 1000,
    "temperature": 0.1,
    "top_p": 1,
    "listCount": 10
  },
  "editor.inlineSuggest.enabled": true,
  "editor.quickSuggestions": {
    "other": true,
    "comments": true,
    "strings": true
  }
}
```

### 3. Verify Copilot Integration

Create a test file to verify Copilot is working:

```csharp
// test-copilot.cs - Delete after verification
using StreamDingo.Core.Models;

namespace StreamDingo.Test;

public class CopilotTest
{
    // Type this comment and let Copilot suggest the implementation:
    // TODO: Create a method that calculates the total watch time for a stream
}
```

If Copilot suggests a method implementation, you're all set!

## Maximizing Copilot Effectiveness

### 1. Use Descriptive Comments
```csharp
// Instead of this:
// Get users

// Use this:
// Retrieve all active users who have streamed in the last 30 days
```

### 2. Leverage TODO Comments
```csharp
// TODO: Implement user authentication with JWT tokens
// TODO: Add rate limiting for API endpoints  
// TODO: Create method to ban abusive users from chat
```

### 3. Write Tests First
```csharp
[Fact]
public void CalculateStreamRevenue_WithValidData_ReturnsCorrectAmount()
{
    // Arrange
    var stream = new StreamTestDataBuilder()
        .WithViewerCount(1000)
        .WithDurationHours(2)
        .Build();
    
    // Act - Let Copilot suggest the implementation after writing this test
    var revenue = _revenueCalculator.CalculateStreamRevenue(stream);
    
    // Assert
    Assert.Equal(expected: 50.00m, actual: revenue);
}
```

### 4. Use Meaningful Variable Names
```csharp
// Good - Copilot understands intent clearly
var activeStreamCount = streams.Where(s => s.IsLive).Count();
var monthlySubscriptionRevenue = CalculateMonthlyRevenue(subscriptions);

// Less effective
var count = streams.Where(s => s.IsLive).Count();
var revenue = Calculate(data);
```

### 5. Context-Rich Method Signatures
```csharp
// Copilot can infer a lot from well-named methods
public async Task<StreamAnalytics> GenerateStreamPerformanceReport(
    Guid streamId, 
    DateTime startDate, 
    DateTime endDate,
    CancellationToken cancellationToken = default)
{
    // Copilot will suggest comprehensive analytics implementation
}
```

## Copilot Chat Usage

### Common Prompts for This Project

1. **Code Generation**
   ```
   Generate a C# method to validate stream title requirements:
   - Must be 3-200 characters
   - No profanity
   - Required field
   ```

2. **Test Generation**
   ```
   Create xUnit tests for the StreamQuality enum covering all values
   ```

3. **Documentation**
   ```
   Generate XML documentation for the IStreamService interface
   ```

4. **Refactoring**
   ```
   Refactor this method to use async/await patterns and improve error handling
   ```

### Project-Specific Context

When asking Copilot questions, provide context:
```
"In the StreamDingo streaming platform project, how would you implement 
real-time viewer count updates using SignalR?"
```

## Troubleshooting

### Copilot Not Providing Suggestions

1. **Check Extension Status**
   - Ensure Copilot extension is enabled and authenticated
   - Check status bar for Copilot indicator

2. **File Type Recognition**
   - Ensure `.cs` files are recognized as C#
   - Check language mode in bottom-right of editor

3. **Clear Context**
   - Sometimes restarting the editor helps
   - Clear Copilot cache if available

### Improving Suggestion Quality

1. **Add More Context**
   ```csharp
   // StreamDingo streaming platform - User management service
   public class UserService : IUserService
   {
       // TODO: Implement user registration with email verification
   }
   ```

2. **Use Project Terminology**
   - Stream, User, Viewer, Category, Quality
   - Copilot learns from consistent terminology

3. **Reference Existing Patterns**
   ```csharp
   // Following the same pattern as CreateStreamAsync in IStreamService
   public async Task<User> CreateUserAsync(string username, string email)
   ```

## Best Practices

### 1. Review All Suggestions
- Copilot suggestions should be reviewed for:
  - Security implications
  - Performance considerations
  - Code style consistency
  - Business logic accuracy

### 2. Combine with Traditional Development
- Use Copilot for boilerplate code
- Apply human judgment for architecture decisions
- Review generated code thoroughly

### 3. Iterative Development
```csharp
// Start with a simple comment
// TODO: Add user authentication

// Refine based on Copilot suggestions
// TODO: Add JWT-based user authentication with role-based authorization

// Further refine
// TODO: Implement JWT authentication with refresh tokens and role-based 
//       authorization supporting streamer, viewer, and admin roles
```

### 4. Maintain Code Quality
- Run `dotnet format` regularly
- Use static analysis tools
- Follow established patterns in the codebase

## Integration with CI/CD

The project includes GitHub Actions workflows that work well with Copilot-generated code:

- **Automated testing** validates Copilot suggestions
- **Code quality checks** ensure standards are maintained
- **Security scanning** catches potential issues

## Resources

- [GitHub Copilot Best Practices](https://docs.github.com/en/copilot/quickstart)
- [.NET Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/)
- [StreamDingo Project Documentation](README.md)

---

**Happy Coding with Copilot! 🚀🤖**