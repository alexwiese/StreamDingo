# StreamDingo

A streaming platform API built with .NET 8 - optimized for development with GitHub Copilot.

[![CI/CD Pipeline](https://github.com/alexwiese/StreamDingo/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/alexwiese/StreamDingo/actions/workflows/ci-cd.yml)

## 🤖 GitHub Copilot Integration

This repository is specifically configured to maximize the effectiveness of GitHub Copilot during development. Here's how to get the most out of Copilot with this project:

### Quick Start with Copilot

1. **Clone and Setup**
   ```bash
   git clone https://github.com/alexwiese/StreamDingo.git
   cd StreamDingo
   dotnet restore
   dotnet build
   ```

2. **Open in Your IDE** with GitHub Copilot enabled:
   - VS Code with GitHub Copilot extension
   - Visual Studio with GitHub Copilot
   - JetBrains Rider with GitHub Copilot plugin

3. **Start Developing** - Copilot is configured to understand:
   - Streaming domain concepts
   - Modern C# patterns
   - ASP.NET Core minimal APIs
   - xUnit testing patterns

### 🚀 Copilot Configuration

This project includes optimized Copilot configuration files:

- **`.copilotrc`** - Project-specific Copilot settings
- **`.github/copilot.yml`** - Advanced Copilot preferences
- **Descriptive comments and TODOs** - Guide Copilot suggestions

### 💡 Copilot-Optimized Code Patterns

The codebase demonstrates patterns that work exceptionally well with GitHub Copilot:

#### 1. Domain Models with Rich Documentation
```csharp
/// <summary>
/// Represents a streaming session in the StreamDingo platform
/// </summary>
public class Stream
{
    /// <summary>
    /// Unique identifier for the stream
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Copilot will suggest additional properties based on context
}
```

#### 2. Service Interfaces with Clear Intent
```csharp
public interface IStreamService
{
    Task<Models.Stream> CreateStreamAsync(string streamerId, string title, 
        string? description = null, CancellationToken cancellationToken = default);
    
    // TODO: Copilot will suggest additional methods for stream management
}
```

#### 3. Test Builders for Complex Objects
```csharp
public class UserTestDataBuilder
{
    public UserTestDataBuilder WithUsername(string username) { /* implementation */ }
    public UserTestDataBuilder WithEmail(string email) { /* implementation */ }
    // Copilot excels at extending builder patterns
}
```

### 🎯 Tips for Using Copilot with This Project

1. **Use Descriptive Comments**
   ```csharp
   // TODO: Add user authentication validation
   // Copilot will suggest authentication logic
   ```

2. **Leverage TODO Comments**
   - Copilot uses TODO comments as context clues
   - See examples throughout the codebase

3. **Write Tests First**
   - Copilot is excellent at implementing methods when tests exist
   - Use the test builders in `StreamDingo.Tests`

4. **Use Meaningful Names**
   - Clear method and variable names help Copilot understand intent
   - Follow established patterns in the codebase

## 🏗️ Project Structure

```
StreamDingo/
├── src/
│   ├── StreamDingo.Core/          # Domain models and services
│   │   ├── Models/                # Domain entities
│   │   └── Services/              # Service interfaces
│   └── StreamDingo.Api/           # REST API (minimal API style)
├── tests/
│   └── StreamDingo.Tests/         # Unit tests with test builders
├── .github/
│   ├── copilot.yml               # Copilot configuration
│   └── workflows/                # CI/CD pipeline
├── .copilotrc                    # Project Copilot settings
└── StreamDingo.sln               # Solution file
```

## 🛠️ Technology Stack

- **.NET 8** - Latest LTS version with modern C# features
- **ASP.NET Core** - Minimal APIs for lightweight REST endpoints
- **xUnit** - Testing framework with excellent Copilot support
- **System.ComponentModel.DataAnnotations** - Validation attributes
- **Swashbuckle** - OpenAPI/Swagger documentation

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- GitHub Copilot subscription and IDE extension

### Running the Application

1. **Start the API**
   ```bash
   dotnet run --project src/StreamDingo.Api
   ```

2. **View Swagger UI**
   - Navigate to `https://localhost:7095/swagger`
   - Explore the API endpoints

3. **Run Tests**
   ```bash
   dotnet test
   ```

## 🧪 Testing with Copilot

The test suite is designed to work seamlessly with Copilot:

```csharp
[Fact]
public void Stream_Should_Initialize_With_Default_Values()
{
    // Arrange & Act
    var stream = new StreamDingo.Core.Models.Stream();
    
    // Assert - Copilot will suggest comprehensive assertions
    Assert.NotEqual(Guid.Empty, stream.Id);
    // More assertions...
}
```

**Pro Tip**: Write the test method name and let Copilot suggest the implementation!

## 📚 API Endpoints

| Endpoint | Method | Description | Copilot Optimization |
|----------|--------|-------------|---------------------|
| `/api/streams` | GET | Get all streams | Returns sample data, ready for service integration |
| `/api/streams/live` | GET | Get live streams | Placeholder for filtering logic |
| `/api/streams/{id}` | GET | Get stream by ID | Includes validation patterns |
| `/api/streams` | POST | Create stream | Demonstrates request/response patterns |
| `/api/users/{id}` | GET | Get user by ID | User management endpoint template |

## 🔮 Next Steps with Copilot

Here are suggested areas where Copilot can help you expand this project:

1. **Database Integration**
   - Add Entity Framework Core
   - Let Copilot suggest DbContext configuration

2. **Authentication & Authorization**
   - Implement JWT authentication
   - Copilot can suggest security patterns

3. **Real-time Features**
   - Add SignalR for live streaming updates
   - Copilot excels at WebSocket implementations

4. **Caching Layer**
   - Implement Redis caching
   - Copilot can suggest caching strategies

5. **File Upload/Streaming**
   - Add video file handling
   - Copilot can suggest media processing pipelines

## 🤝 Contributing with Copilot

1. **Fork the repository**
2. **Create a feature branch** with descriptive names
3. **Use TODO comments** to guide your development
4. **Let Copilot suggest implementations** based on the established patterns
5. **Write tests first** - Copilot will help implement the functionality
6. **Submit a pull request** with clear descriptions

## 📖 Learning Resources

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Minimal APIs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [xUnit Testing Patterns](https://xunit.net/)

---

**Made with ❤️ and 🤖 GitHub Copilot**

This repository showcases how to structure a .NET project to maximize GitHub Copilot's effectiveness. The combination of clear documentation, meaningful names, and thoughtful architecture creates an ideal environment for AI-assisted development.