
# StreamDingo - Event Sourcing .NET Library

StreamDingo is a .NET library for event sourcing that enables snapshot-based event replay with integrity verification using hash-based validation. The library leverages the alexwiese/hashstamp library for generating and verifying hashes of event handlers and snapshots.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Initial Project Setup (Greenfield)
If the repository contains only planning documents and no source code:

- **Bootstrap a new .NET library project:**
  ```bash
  dotnet new classlib -n StreamDingo -f net8.0
  dotnet new sln -n StreamDingo
  dotnet sln add StreamDingo/StreamDingo.csproj
  ```

- **Create test project:**
  ```bash
  dotnet new xunit -n StreamDingo.Tests -f net8.0
  dotnet sln add StreamDingo.Tests/StreamDingo.Tests.csproj
  cd StreamDingo.Tests
  dotnet add reference ../StreamDingo/StreamDingo.csproj
  ```

- **Add required dependencies (once project exists):**
  ```bash
  cd StreamDingo
  dotnet add package System.Text.Json
  # Add alexwiese/hashstamp when available on NuGet
  ```

### Build and Test Commands

- **Build the solution:**
  ```bash
  dotnet build
  ```
  - Takes approximately 10-15 seconds. NEVER CANCEL. Set timeout to 2+ minutes.

- **Run all tests:**
  ```bash
  dotnet test
  ```
  - Takes approximately 5-10 seconds for basic tests. NEVER CANCEL. Set timeout to 2+ minutes.
  - For comprehensive test suites with integration tests, may take 1-5 minutes. Set timeout to 10+ minutes.

- **Create NuGet package:**
  ```bash
  dotnet pack --configuration Release
  ```
  - Takes approximately 15-30 seconds. NEVER CANCEL. Set timeout to 2+ minutes.

- **Restore dependencies:**
  ```bash
  dotnet restore
  ```
  - Takes approximately 5-20 seconds depending on dependencies. NEVER CANCEL. Set timeout to 2+ minutes.

### Development Environment Setup

- **Prerequisites:** .NET 8.0 SDK (verified available: .NET 8.0.119)
- **IDE:** Compatible with Visual Studio 2022, Visual Studio Code, or Rider
- **Git:** Required for version control (verified available)

### Validation Scenarios

ALWAYS test these scenarios after making changes to ensure the event sourcing functionality works correctly:

1. **Basic Event Handler Testing:**
   ```csharp
   // Create a simple event and handler
   var initialSnapshot = new TestSnapshot();
   var testEvent = new TestEvent();
   var handler = new TestEventHandler();
   
   var resultSnapshot = handler.Apply(initialSnapshot, testEvent);
   
   // Verify snapshot hash integrity
   Assert.NotNull(resultSnapshot);
   Assert.NotEqual(initialSnapshot.Hash, resultSnapshot.Hash);
   ```

2. **Event Replay Scenario:**
   ```csharp
   // Test event ordering changes and replay functionality
   var events = new List<IEvent> { event1, event2, event3 };
   var reorderedEvents = new List<IEvent> { event1, event3, event2 };
   
   var originalResult = EventSourcing.ReplayEvents(initialSnapshot, events);
   var reorderedResult = EventSourcing.ReplayEvents(initialSnapshot, reorderedEvents);
   
   // Results should be different, triggering replay mechanism
   ```

3. **Hash Verification Scenario:**
   ```csharp
   // Test that hash changes in event handlers trigger replay
   var handlerV1 = new TestEventHandler(); // Version 1
   var handlerV2 = new TestEventHandlerModified(); // Version 2 with different logic
   
   // Verify that hash differences are detected and replay is triggered
   ```

### Running the Library

Since this is a library project and not an application:

- **Cannot run directly** - this is a class library, not an executable application
- **Test the library** using the test project: `dotnet test`
- **Use the library** by referencing it in other .NET projects
- **Package the library** for distribution: `dotnet pack`

### Common Tasks

#### Code Quality and Style
- **Format code:**
  ```bash
  dotnet format
  ```
- **Analyze code (if analyzers are configured):**
  ```bash
  dotnet build --verbosity normal
  ```

#### Package Management
- **Add new package:**
  ```bash
  dotnet add package <PackageName>
  ```
- **Remove package:**
  ```bash
  dotnet remove package <PackageName>
  ```
- **List packages:**
  ```bash
  dotnet list package
  ```

#### Version Management
- **Update project version** in StreamDingo.csproj:
  ```xml
  <PropertyGroup>
    <Version>1.0.0</Version>
    <PackageVersion>1.0.0</PackageVersion>
  </PropertyGroup>
  ```

### Project Structure (Once Implemented)

Expected project layout:
```
StreamDingo/
├── src/
│   └── StreamDingo/
│       ├── EventSourcing/
│       ├── Snapshots/
│       ├── Hashing/
│       └── StreamDingo.csproj
├── tests/
│   └── StreamDingo.Tests/
│       ├── EventSourcing/
│       ├── Integration/
│       └── StreamDingo.Tests.csproj
├── StreamDingo.sln
├── README.md
└── .github/
    └── workflows/
        └── ci.yml
```

### Key Implementation Areas

Based on `plan/implementation_plan.md`, focus on these core components:

1. **Event Handlers**: Each takes previous snapshot, applies logic, generates mutated snapshot
2. **Hash Management**: Using alexwiese/hashstamp for event handler code hashing
3. **Snapshot Management**: Each snapshot has a hash, last snapshot is current entity value
4. **Event Replay Logic**: Handle event order changes and code hash changes
5. **Integrity Verification**: Verify integrity of code, diffs, and snapshots

### Important Notes

- **Development Status**: This is currently a greenfield project with only planning documents
- **Dependencies**: Will require alexwiese/hashstamp library (check if available on NuGet)
- **Architecture**: Event sourcing library focusing on snapshot-based replay with hash verification
- **Testing Strategy**: Emphasize testing event replay scenarios and hash integrity
- **No CI/CD**: No GitHub Actions workflows exist yet - will need to be created

### Troubleshooting

Common issues and solutions:

1. **Build fails**: Run `dotnet restore` first, then `dotnet build`
2. **Tests fail**: Ensure project references are correct in test project
3. **Package creation fails**: Verify project metadata in .csproj file
4. **Hash verification issues**: Ensure alexwiese/hashstamp is properly referenced

### Reference Files

- Use `.github/csharp.instructions.md` for C#-specific coding guidelines
- Use `.github/devops.instructions.md` for DevOps principles and CI/CD guidance
- Refer to `plan/implementation_plan.md` for detailed architecture and requirements

### Timing Expectations

- **Project creation**: 10-30 seconds per template
- **Build operations**: 10-30 seconds for clean builds
- **Test execution**: 5 seconds to 5 minutes depending on test complexity
- **Package creation**: 15-30 seconds
- **Dependency restoration**: 5-60 seconds depending on cache and network

**CRITICAL**: NEVER CANCEL any build, test, or package operation. Always set timeouts of at least 2+ minutes for basic operations and 10+ minutes for comprehensive test suites.
