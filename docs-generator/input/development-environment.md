---
Layout: _Layout
Title: development-environment
---
---
---
# StreamDingo - Development Environment Setup

> **Complete guide for setting up a StreamDingo development environment**

## 🎯 Prerequisites

### Required Software

#### .NET SDK
- **Version**: .NET 9.0.304 or later
- **Download**: [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Verification**: Run `dotnet --version` to confirm installation

```bash
# Install .NET 9.0 SDK (Linux/macOS)
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0 --install-dir ~/.dotnet
export PATH="$HOME/.dotnet:$PATH"

# Verify installation
dotnet --version  # Should show 9.0.304 or later
```

#### Git
- **Version**: Git 2.30 or later
- **Download**: [https://git-scm.com/downloads](https://git-scm.com/downloads)
- **Verification**: Run `git --version` to confirm installation

### Recommended IDEs

#### Visual Studio 2022 (Windows)
- **Version**: 17.8 or later
- **Workload**: .NET desktop development
- **Extensions**:
  - GitHub Copilot
  - SonarLint for Visual Studio
  - EditorConfig Language Service

#### Visual Studio Code (Cross-Platform)
- **Version**: 1.85 or later
- **Extensions**:
  - C# Dev Kit
  - GitHub Copilot
  - EditorConfig for VS Code
  - GitLens
  - Coverage Gutters

#### JetBrains Rider (Cross-Platform)
- **Version**: 2023.3 or later
- **Plugins**:
  - GitHub Copilot
  - SonarLint
  - Code Coverage

## 🚀 Quick Setup

### 1. Clone Repository
```bash
git clone https://github.com/alexwiese/StreamDingo.git
cd StreamDingo
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build Solution
```bash
dotnet build src/StreamDingo/StreamDingo.csproj
```

### 4. Run Tests
```bash
dotnet test tests/StreamDingo.Tests/StreamDingo.Tests.csproj
```

### 5. Format Code
```bash
dotnet format
```

## 🛠️ Development Workflow

### Daily Development Commands

#### Build and Test
```bash
# Full build and test cycle
dotnet restore && dotnet build && dotnet test

# Quick test run
dotnet test --no-build

# Build with verbose output for debugging
dotnet build --verbosity normal
```

#### Code Quality
```bash
# Format code according to .editorconfig
dotnet format

# Check formatting without making changes
dotnet format --verify-no-changes

# Run static analysis (if configured)
dotnet build --verbosity normal
```

#### Package Creation
```bash
# Create development package
dotnet pack src/StreamDingo/StreamDingo.csproj --configuration Debug

# Create release package
dotnet pack src/StreamDingo/StreamDingo.csproj --configuration Release
```

### Testing Workflow

#### Unit Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults/

# Run specific test class
dotnet test --filter "FullyQualifiedName~StreamDingo.Tests.EventSourcingTests"

# Run tests with detailed output
dotnet test --verbosity normal
```

#### Integration Tests
```bash
# Run integration tests (when implemented)
dotnet test --filter "Category=Integration"

# Run with specific test settings
dotnet test --settings tests.runsettings
```

### Performance Testing

#### Benchmarks
```bash
# Run benchmarks (when implemented)
dotnet run --project benchmarks/StreamDingo.Benchmarks --configuration Release

# Run specific benchmark category
dotnet run --project benchmarks/StreamDingo.Benchmarks --configuration Release -- --filter "*EventAppend*"
```

## 🔧 Project Structure

### Solution Layout
```
StreamDingo/
├── src/
│   └── StreamDingo/              # Main library
├── tests/
│   └── StreamDingo.Tests/        # Unit and integration tests
├── benchmarks/
│   └── StreamDingo.Benchmarks/   # Performance benchmarks
├── examples/
│   └── StreamDingo.Examples/     # Usage examples
├── docs/                         # Documentation source
├── plan/                         # Implementation planning
└── assets/                       # Images and diagrams
```

### Key Configuration Files

#### global.json
- Specifies required .NET SDK version
- Ensures consistent builds across environments

#### .editorconfig
- Defines code formatting rules
- Enforces consistent style across team
- Integrated with IDE formatting tools

#### StreamDingo.slnx
- Modern solution file format
- Faster loading and better performance
- Enhanced project dependency management

## 🧪 Testing Framework

### Test Technologies
- **xUnit**: Primary testing framework
- **FluentAssertions**: Readable assertion syntax
- **Moq**: Mocking framework for unit tests
- **Coverlet**: Code coverage collection
- **BenchmarkDotNet**: Performance benchmarking

### Test Categories
```csharp
// Unit tests
[Fact]
public void EventHandler_Should_ProcessEvent()
{
    // Arrange, Act, Assert
}

// Integration tests
[Fact]
[Trait("Category", "Integration")]
public async Task EventStore_Should_PersistEvents()
{
    // Full integration test
}

// Performance tests  
[Benchmark]
public void EventAppend_Performance()
{
    // Performance measurement
}
```

### Coverage Requirements
- **Minimum**: 90% line coverage
- **Target**: 95% line coverage
- **Critical Paths**: 100% coverage required
- **Exclusions**: Generated code, trivial properties

## 🔍 Code Quality Tools

### Static Analysis
- **Built-in**: Roslyn analyzers enabled
- **FxCop**: Microsoft.CodeAnalysis.NetAnalyzers
- **Security**: Microsoft.CodeAnalysis.BannedApiAnalyzers
- **Performance**: Custom analyzers for allocation detection

### Code Formatting
- **Tool**: `dotnet format`
- **Configuration**: `.editorconfig`
- **Enforcement**: CI/CD pipeline validation
- **IDE Integration**: Format on save recommended

### Documentation
- **XML Comments**: Required for all public APIs
- **Style**: Microsoft documentation standards
- **Generation**: DocFX or similar tool
- **Examples**: Include usage examples in XML comments

## 🚀 CI/CD Integration

### GitHub Actions Workflows

#### Continuous Integration (`ci.yml`)
- Build validation on all pushes and PRs
- Test execution with coverage reporting
- Code formatting verification
- Security vulnerability scanning

#### Documentation (`docs.yml`)  
- MkDocs site generation and deployment
- API documentation building
- GitHub Pages publishing
- Link validation and broken link detection

#### Performance (`pr-benchmark.yml`)
- Automated benchmark execution on PRs
- Performance regression detection
- Benchmark result comparison and reporting
- Performance trend tracking

#### Release (`release.yml`)
- Automated release creation on tags
- NuGet package publishing
- Release notes generation
- Asset upload and distribution

### Local Development Integration
```bash
# Simulate CI locally
./.github/scripts/validate-pr.sh

# Run all checks
dotnet restore && dotnet build && dotnet test && dotnet format --verify-no-changes
```

## 🐛 Debugging and Troubleshooting

### Common Issues

#### .NET SDK Version Mismatch
```bash
# Check current SDK
dotnet --version

# Check available SDKs  
dotnet --list-sdks

# Install specific version
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 9.0.304
```

#### Build Failures
```bash
# Clean and rebuild
dotnet clean && dotnet restore && dotnet build

# Verbose build output
dotnet build --verbosity diagnostic

# Check for package conflicts
dotnet list package --outdated
```

#### Test Failures
```bash
# Run specific failing test
dotnet test --filter "FullyQualifiedName=StreamDingo.Tests.SpecificTest"

# Debug test output
dotnet test --verbosity normal --logger "console;verbosity=detailed"

# Collect crash dumps (if tests crash)
dotnet test --collect:"Crash Dump"
```

### Performance Debugging
```bash
# Profile memory usage
dotnet-trace collect --providers Microsoft-DotNETCore-SampleProfiler --process-id <PID>

# Analyze garbage collection
dotnet-trace collect --providers Microsoft-Windows-DotNETRuntime:gc --process-id <PID>

# Monitor allocations
dotnet-counters monitor --process-id <PID> System.Runtime
```

## 📚 Learning Resources

### Event Sourcing Concepts
- [Event Sourcing Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing)
- [CQRS and Event Sourcing](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [DDD and Event Sourcing](https://www.eventstore.com/blog/what-is-event-sourcing)

### .NET Performance
- [.NET Performance Best Practices](https://docs.microsoft.com/en-us/dotnet/framework/performance/performance-tips)
- [Memory Management and GC](https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/)
- [Async Programming Patterns](https://docs.microsoft.com/en-us/dotnet/csharp/async)

### Testing and Quality
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Guide](https://fluentassertions.com/introduction)

## 🎯 Development Tips

### Performance Best Practices
- Use `Span<T>` and `Memory<T>` for buffer operations
- Avoid `async void` except for event handlers
- Use `ConfigureAwait(false)` in library code
- Profile regularly with BenchmarkDotNet

### Code Organization
- Group related functionality in folders
- Use meaningful namespace hierarchies
- Keep classes focused and single-purpose
- Prefer composition over inheritance

### Testing Strategies
- Write tests first for complex logic
- Use descriptive test method names
- Test both happy path and error scenarios
- Mock external dependencies appropriately

---

**Last Updated**: $(date)  
**Version**: 1.0  
**Status**: Complete
