
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

**VALIDATED COMMANDS** - All commands below have been tested and measured:

- **Restore dependencies (REQUIRED FIRST):**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet restore
  ```
  - Takes 33 seconds (first time with downloads), 1-2 seconds after cache. NEVER CANCEL. Set timeout to 5+ minutes.

- **Build the solution:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet build --configuration Release
  ```
  - Takes 8 seconds after restore. NEVER CANCEL. Set timeout to 5+ minutes.

- **Run all tests:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet test --configuration Release
  ```
  - Takes 3 seconds (9 tests currently). NEVER CANCEL. Set timeout to 10+ minutes.

- **Format code:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet format --verify-no-changes
  ```
  - Takes 9 seconds. NEVER CANCEL. Set timeout to 5+ minutes.

- **Create NuGet package:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet pack --configuration Release --output ./artifacts
  ```
  - Takes 2 seconds. NEVER CANCEL. Set timeout to 5+ minutes.

### Running the Library

Since this is a library project and not an application:

- **Cannot run directly** - this is a class library, not an executable application
- **Test the library** using the test project: `export PATH="$HOME/.dotnet:$PATH" && dotnet test --configuration Release`
- **Use the library** by referencing it in other .NET projects
- **Package the library** for distribution: `export PATH="$HOME/.dotnet:$PATH" && dotnet pack --configuration Release --output ./artifacts`

### Running Examples for Validation

**Console Example (VERIFIED WORKING):**
```bash
export PATH="$HOME/.dotnet:$PATH" && dotnet run --project examples/StreamDingo.Examples/StreamDingo.Examples.csproj --configuration Release
```
- Takes 2-3 seconds. Shows "StreamDingo core library successfully initialized"

**WebApp Example Backend (VERIFIED WORKING):**
```bash
cd examples/StreamDingo.Examples.WebApp/Backend
export PATH="$HOME/.dotnet:$PATH" && dotnet run --urls "http://localhost:5000"
```
- Test with: `curl http://localhost:5000/api/users` (should return empty array `[]`)

**WebApp Example Frontend (VERIFIED WORKING):**
```bash
cd examples/StreamDingo.Examples.WebApp/Frontend
npm install && npm run build
```
- npm install takes 9 seconds, npm run build takes 4 seconds

### Validation Scenarios

ALWAYS test these scenarios after making changes to ensure the event sourcing functionality works correctly:

1. **Basic Build and Test Flow**:
   ```bash
   export PATH="$HOME/.dotnet:$PATH"
   dotnet restore  # 33s first time, 1-2s cached
   dotnet build --configuration Release  # 8s 
   dotnet test --configuration Release  # 3s, 9 tests should pass
   dotnet format --verify-no-changes  # 9s, should pass with no changes
   ```

2. **Example Validation**:
   ```bash
   export PATH="$HOME/.dotnet:$PATH"
   dotnet run --project examples/StreamDingo.Examples/StreamDingo.Examples.csproj --configuration Release
   ```
   - Should output "StreamDingo core library successfully initialized"

3. **Package Creation**:
   ```bash
   export PATH="$HOME/.dotnet:$PATH"
   dotnet pack --configuration Release --output ./artifacts
   ls artifacts/  # Should show StreamDingo.1.0.0.nupkg
   ```

4. **Event Sourcing Flow** (when implemented):
   - Create event handlers that take previous snapshot + event → new snapshot
   - Append events to streams
   - Replay events from beginning to rebuild current state
   - Verify snapshots are created correctly

5. **Hash Integrity Verification** (when implemented):
   - Event handler code changes should be detected via hash comparison
   - Snapshot hashes should verify data integrity
   - Event hashes should prevent tampering

6. **Event Replay Scenarios** (when implemented):
   - Event order changes should trigger replay from last stable snapshot
   - Event handler code changes should replay affected event types
   - Replay should stop when snapshot hashes stabilize

7. **Snapshot Management** (when implemented):
   - Automatic snapshots should be created at configured intervals
   - Manual snapshot creation should work correctly
   - Last snapshot should represent current entity state

### Common Tasks

#### Code Quality and Style
- **Format code:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet format
  ```
  - Takes 9 seconds. NEVER CANCEL.
- **Verify formatting:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet format --verify-no-changes
  ```
  - Takes 9 seconds. Should pass with no changes required.
- **Analyze code (if analyzers are configured):**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet build --verbosity normal
  ```

#### Package Management
- **Add new package:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet add src/StreamDingo package <package-name>
  ```
- **Remove package:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet remove src/StreamDingo package <package-name>
  ```
- **List packages:**
  ```bash
  export PATH="$HOME/.dotnet:$PATH" && dotnet list src/StreamDingo package
  ```

#### Version Management
- **Update project version** in `src/StreamDingo/StreamDingo.csproj`:
  ```xml
  <Version>1.0.1</Version>
  ```

### Project Structure

Current project layout (VERIFIED):
```
StreamDingo/
├── src/
│   └── StreamDingo/
│       ├── StreamDingo.csproj
│       └── [Core library implementation files]
├── tests/
│   └── StreamDingo.Tests/
│       ├── StreamDingo.Tests.csproj
│       └── [9 xUnit tests - all passing]
├── examples/
│   ├── StreamDingo.Examples/
│   │   ├── Program.cs (working console example)
│   │   └── StreamDingo.Examples.csproj
│   └── StreamDingo.Examples.WebApp/
│       ├── Backend/ (ASP.NET Core API - working)
│       └── Frontend/ (React+TypeScript+Vite - working)
├── docs/ (comprehensive documentation)
├── benchmarks/ (BenchmarkDotNet performance tests)
├── docs-generator/ (documentation generation)
├── README.md (comprehensive documentation)
├── global.json (.NET 9.0.304 SDK requirement)
├── .editorconfig (comprehensive C# formatting rules)
├── StreamDingo.slnx (solution file)
└── .github/
    ├── copilot-instructions.md
    └── workflows/ (CI/CD pipelines)
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

- **Current Status**: Fully functional library with core implementations and working examples
- **Dependencies**: Uses standard .NET dependencies (no external event sourcing libraries required)
- **Architecture**: Event sourcing library focusing on snapshot-based replay with hash verification
- **Testing Strategy**: 9 xUnit tests currently passing - emphasize testing event replay scenarios and hash integrity
- **.NET Version**: Currently targeting .NET 9.0 (latest version for best performance and features)
- **Always run `export PATH="$HOME/.dotnet:$PATH" && dotnet format` after making code changes**

### Troubleshooting

Common issues and solutions:

1. **Build fails**: Run `export PATH="$HOME/.dotnet:$PATH" && dotnet restore` first, then `export PATH="$HOME/.dotnet:$PATH" && dotnet build --configuration Release`
2. **Tests fail**: Ensure project references are correct in test project. Current tests should all pass (9/9).
3. **Package creation fails**: Verify project metadata in .csproj file
4. **"dotnet command not found"**: Install .NET 9.0 SDK using the installation command above
5. **.NET Version Issues**: Ensure .NET 9.0 SDK is installed and global.json matches available SDK version

### CI/CD Pipeline Information

The repository has GitHub Actions workflows in `.github/workflows/`:

- **ci.yml**: Main CI pipeline that runs on push/PR
  - Builds solution (Release configuration)
  - Runs tests with coverage
  - Performs format checking (`dotnet format --verify-no-changes`)
  - Security scanning
  - Creates NuGet packages
  - **Expected to pass** - all commands have been validated

- **docs.yml**: Documentation generation
- **release.yml**: Release automation  
- **pr-benchmark.yml**: Performance benchmarks on PRs

**CI Validation Commands** - these must pass for CI success:
```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet restore
dotnet build --no-restore --configuration Release --verbosity normal
dotnet test --no-build --configuration Release --verbosity normal
dotnet format --verify-no-changes --verbosity diagnostic
dotnet pack --no-restore --configuration Release --output ./artifacts
```

### Reference Files

- Use `.github/csharp.instructions.md` for C#-specific coding guidelines
- Use `.github/devops.instructions.md` for DevOps principles and CI/CD guidance
- Refer to `plan/implementation_plan.md` for detailed architecture and requirements
- Follow the comprehensive README.md for user-facing documentation standards

### Timing Expectations (MEASURED)

**Actual measured times from validation:**
- **.NET 9.0 SDK installation**: 30-60 seconds
- **First dotnet restore**: 33 seconds (with downloads)
- **Cached dotnet restore**: 1-2 seconds  
- **dotnet build**: 8 seconds
- **dotnet test**: 3 seconds (9 tests)
- **dotnet format**: 9 seconds
- **dotnet pack**: 2 seconds
- **npm install (frontend)**: 9 seconds
- **npm run build (frontend)**: 4 seconds
- **Console example run**: 2-3 seconds

**CRITICAL**: NEVER CANCEL any build, test, or package operation. Always set timeouts of at least 5+ minutes for basic operations and 10+ minutes for comprehensive test suites.

## Common Tasks

### Pre-commit Validation Workflow

ALWAYS run this complete workflow before committing changes:

```bash
# Ensure .NET 9.0 is available
export PATH="$HOME/.dotnet:$PATH"

# 1. Restore dependencies (33s first time, 1-2s cached)
dotnet restore

# 2. Build solution (8s)
dotnet build --configuration Release

# 3. Run all tests (3s, 9 tests should pass)  
dotnet test --configuration Release

# 4. Verify code formatting (9s)
dotnet format --verify-no-changes

# 5. Create package to verify it builds (2s)
dotnet pack --configuration Release --output ./artifacts

# 6. Test examples to verify functionality (2-3s)
dotnet run --project examples/StreamDingo.Examples/StreamDingo.Examples.csproj --configuration Release
```

Total time: ~55 seconds first time, ~25 seconds with cache.
