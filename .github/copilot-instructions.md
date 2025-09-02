
# StreamDingo - Event Sourcing .NET Library

StreamDingo is a .NET library for event sourcing that enables snapshot-based event replay with integrity verification using hash-based validation. The library leverages the alexwiese/hashstamp library for generating and verifying hashes of event handlers and snapshots.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Current Project Status
The project has the following structure:
- **Main Library**: `src/StreamDingo/` - Core event sourcing library
- **Tests**: `tests/StreamDingo.Tests/` - Unit and integration tests
- **Examples**: `examples/StreamDingo.Examples/` - Usage examples
- **Documentation**: Comprehensive README and API documentation

### Development Environment Setup

- **Prerequisites:** .NET 9.0 SDK (currently using 9.0.304)
- **IDE:** Compatible with Visual Studio 2022, Visual Studio Code, or Rider
- **Git:** Required for version control (verified available)

**CRITICAL .NET 9 SDK REQUIREMENT:**
- **Always install .NET 9.0 latest SDK if not present** before trying to run any dotnet commands
- **Installation command:**
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0 --install-dir ~/.dotnet
  export PATH="$HOME/.dotnet:$PATH"
  ```
- **Verification:** Run `dotnet --version` to confirm 9.0.x is available
- **Required for all operations:** build, test, pack, restore, format

### Build and Test Commands

- **Build the solution:**
  ```bash
  dotnet build src/StreamDingo/StreamDingo.csproj
  ```
  - Takes approximately 10-15 seconds. NEVER CANCEL. Set timeout to 2+ minutes.

- **Run all tests:**
  ```bash
  dotnet test tests/StreamDingo.Tests/StreamDingo.Tests.csproj
  ```
  - Takes approximately 5-10 seconds for basic tests. NEVER CANCEL. Set timeout to 2+ minutes.
  - For comprehensive test suites with integration tests, may take 1-5 minutes. Set timeout to 10+ minutes.

- **Create NuGet package:**
  ```bash
  dotnet pack src/StreamDingo/StreamDingo.csproj --configuration Release
  ```
  - Takes approximately 15-30 seconds. NEVER CANCEL. Set timeout to 2+ minutes.

- **Restore dependencies:**
  ```bash
  dotnet restore
  ```
  - Takes approximately 5-20 seconds depending on dependencies. NEVER CANCEL. Set timeout to 2+ minutes.

### Running the Library

Since this is a library project and not an application:

- **Cannot run directly** - this is a class library, not an executable application
- **Test the library** using the test project: `dotnet test tests/StreamDingo.Tests/StreamDingo.Tests.csproj`
- **Use the library** by referencing it in other .NET projects
- **Package the library** for distribution: `dotnet pack src/StreamDingo/StreamDingo.csproj --configuration Release`

### Validation Scenarios

ALWAYS test these scenarios after making changes to ensure the event sourcing functionality works correctly:

1. **Basic Event Sourcing Flow**:
   - Create event handlers that take previous snapshot + event → new snapshot
   - Append events to streams
   - Replay events from beginning to rebuild current state
   - Verify snapshots are created correctly

2. **Hash Integrity Verification**:
   - Event handler code changes should be detected via hash comparison
   - Snapshot hashes should verify data integrity
   - Event hashes should prevent tampering

3. **Event Replay Scenarios**:
   - Event order changes should trigger replay from last stable snapshot
   - Event handler code changes should replay affected event types
   - Replay should stop when snapshot hashes stabilize

4. **Snapshot Management**:
   - Automatic snapshots should be created at configured intervals
   - Manual snapshot creation should work correctly
   - Last snapshot should represent current entity state

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
  dotnet add src/StreamDingo package <PackageName>
  ```
- **Remove package:**
  ```bash
  dotnet remove src/StreamDingo package <PackageName>
  ```
- **List packages:**
  ```bash
  dotnet list src/StreamDingo package
  ```

#### Version Management
- **Update project version** in `src/StreamDingo/StreamDingo.csproj`:
  ```xml
  <PropertyGroup>
    <Version>1.0.0</Version>
    <PackageVersion>1.0.0</PackageVersion>
  </PropertyGroup>
  ```

### Project Structure

Current project layout:
```
StreamDingo/
├── src/
│   └── StreamDingo/
│       ├── Library.cs (placeholder - to be replaced)
│       └── StreamDingo.csproj
├── tests/
│   └── StreamDingo.Tests/
│       ├── GlobalUsings.cs
│       ├── LibraryTests.cs
│       └── StreamDingo.Tests.csproj
├── examples/
│   └── StreamDingo.Examples/
│       └── StreamDingo.Examples.csproj
├── README.md (comprehensive documentation)
├── global.json (.NET 9.0 SDK configuration)
├── .editorconfig (comprehensive C# formatting rules)
└── .github/
    ├── copilot-instructions.md
    ├── csharp.instructions.md
    └── devops.instructions.md
```

### Key Implementation Areas

Based on `plan/implementation_plan.md`, focus on these core components:

1. **Event Handlers**: Pure functions that take (previous snapshot, event) → new snapshot
2. **Hash Management**: Using alexwiese/hashstamp for event handler code hashing
3. **Snapshot Management**: Each snapshot has a hash, last snapshot is current entity value
4. **Event Replay Logic**: Handle event order changes and code hash changes
5. **Integrity Verification**: Verify integrity of code, diffs, and snapshots
6. **Event Store**: Core storage and retrieval of events and snapshots

### Important Notes

- **Current Status**: Basic project structure with placeholder code, ready for implementation
- **Dependencies**: Will require alexwiese/hashstamp library (check if available on NuGet)
- **Architecture**: Event sourcing library focusing on snapshot-based replay with hash verification
- **Testing Strategy**: Emphasize testing event replay scenarios and hash integrity
- **.NET Version**: Currently targeting .NET 9.0 (latest version for best performance and features)
- **Always run dotnet format after making code changes**

### Troubleshooting

Common issues and solutions:

1. **Build fails**: Run `dotnet restore` first, then `dotnet build src/StreamDingo/StreamDingo.csproj`
2. **Tests fail**: Ensure project references are correct in test project
3. **Package creation fails**: Verify project metadata in .csproj file
4. **Hash verification issues**: Ensure alexwiese/hashstamp is properly referenced
5. **.NET Version Issues**: Ensure .NET 9.0 SDK is installed and global.json matches available SDK version

### Reference Files

- Use `.github/csharp.instructions.md` for C#-specific coding guidelines
- Use `.github/devops.instructions.md` for DevOps principles and CI/CD guidance
- Refer to `plan/implementation_plan.md` for detailed architecture and requirements
- Follow the comprehensive README.md for user-facing documentation standards

### Timing Expectations

- **Project creation**: 10-30 seconds per template
- **Build operations**: 10-30 seconds for clean builds
- **Test execution**: 5 seconds to 5 minutes depending on test complexity
- **Package creation**: 15-30 seconds
- **Dependency restoration**: 5-60 seconds depending on cache and network

**CRITICAL**: NEVER CANCEL any build, test, or package operation. Always set timeouts of at least 2+ minutes for basic operations and 10+ minutes for comprehensive test suites.
