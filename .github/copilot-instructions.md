# StreamDingo

StreamDingo is a .NET 8.0 web API application designed for streaming functionality. The solution follows a clean architecture pattern with Core business logic, Web API presentation layer, and comprehensive test coverage.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Working Effectively

### Bootstrap and Build
- Bootstrap the repository:
  - `dotnet --version` -- Verify .NET 8.0.119 is installed
  - `dotnet restore` -- Restore NuGet packages (takes 5-10 seconds)
  - `dotnet build` -- Initial build (takes 10-15 seconds). **NEVER CANCEL: Set timeout to 30+ minutes for safety**
- Clean rebuild when needed:
  - `dotnet clean && dotnet build` -- Full rebuild (takes 8-12 seconds total). **NEVER CANCEL: Set timeout to 30+ minutes**
- Release builds:
  - `dotnet build --configuration Release` -- Takes 10-15 seconds. **NEVER CANCEL: Set timeout to 30+ minutes**

### Testing
- Run all tests:
  - `dotnet test` -- Runs xUnit tests (takes 6-8 seconds). **NEVER CANCEL: Set timeout to 15+ minutes**
  - Tests are located in `tests/StreamDingo.Tests/`
  - Uses xUnit framework with Assert library
- Test with coverage:
  - `dotnet test --collect:"XPlat Code Coverage"` -- If coverage tools are needed

### Running the Application
- **Development server (API)**:
  - `dotnet run --project src/StreamDingo.Api/StreamDingo.Api.csproj`
  - Application starts at `http://localhost:5155`
  - Swagger UI available at `http://localhost:5155/swagger/index.html`
  - **IMPORTANT**: The application runs in Development mode by default with Swagger enabled

### Code Quality and Formatting
- **ALWAYS run before committing**:
  - `dotnet format` -- Format all code according to .NET standards (takes 8-10 seconds)
  - `dotnet format --verify-no-changes` -- Verify formatting compliance (fails if formatting needed)
- Code formatting is enforced - the CI will fail if code is not properly formatted

## Validation Scenarios

**ALWAYS test these scenarios after making changes to ensure the application works correctly:**

1. **Build and Test Workflow**:
   - Run `dotnet build` and verify no errors
   - Run `dotnet test` and verify all tests pass
   - Run `dotnet format --verify-no-changes` to ensure proper formatting

2. **API Functionality**:
   - Start the API with `dotnet run --project src/StreamDingo.Api/StreamDingo.Api.csproj`
   - Test WeatherForecast endpoint: `curl http://localhost:5155/weatherforecast`
   - Test Streams endpoint: `curl http://localhost:5155/streams/status`
   - Verify both return HTTP 200 and valid JSON responses
   - Check Swagger UI loads at `http://localhost:5155/swagger/index.html`

3. **Service Integration**:
   - Verify StreamService in Core library functions correctly
   - Test dependency injection works in API controllers
   - Ensure new endpoints return expected data format

## Repository Structure

### Key Projects
- **`src/StreamDingo.Api/`** - ASP.NET Core Web API project
  - Main application entry point (`Program.cs`)
  - API endpoints and controllers
  - Swagger/OpenAPI configuration
- **`src/StreamDingo.Core/`** - Core business logic library
  - Domain services and models
  - Business logic implementation
- **`tests/StreamDingo.Tests/`** - xUnit test project
  - Unit tests for Core library
  - Integration tests for API endpoints

### Project Dependencies
- StreamDingo.Api → StreamDingo.Core
- StreamDingo.Tests → StreamDingo.Core

### Key Files
- **`StreamDingo.sln`** - Solution file containing all projects
- **`src/StreamDingo.Api/Program.cs`** - API application startup and configuration
- **`src/StreamDingo.Core/Class1.cs`** - Core streaming service implementation (contains StreamService class)
- **`tests/StreamDingo.Tests/UnitTest1.cs`** - Unit tests for streaming functionality (contains StreamServiceTests class)

## Development Environment

### Required Software
- .NET 8.0 SDK (8.0.119 confirmed working)
  - Pre-installed at `/usr/bin/dotnet`
  - Includes MSBuild version 17.8.32+74df0b3f5
  - Includes `dotnet format` tool version 8.1.631901

### Available Templates
Common dotnet new templates available:
- `console` - Console applications
- `webapi` - ASP.NET Core Web API
- `classlib` - Class libraries  
- `xunit` - xUnit test projects
- `sln` - Solution files

## Build Timings and Timeouts

**CRITICAL: NEVER CANCEL builds or tests early. Always use these timeout values:**

- **Initial build**: 10-15 seconds actual, **set timeout to 30+ minutes**
- **Clean rebuild**: 8-12 seconds actual, **set timeout to 30+ minutes**
- **Release build**: 10-15 seconds actual, **set timeout to 30+ minutes**
- **Test execution**: 6-8 seconds actual, **set timeout to 15+ minutes**
- **Code formatting**: 8-10 seconds actual, set timeout to 5+ minutes
- **Package restore**: 5-10 seconds actual, set timeout to 10+ minutes

## Common Commands Reference

### Frequently Used Commands
```bash
# Bootstrap
dotnet restore
dotnet build

# Development workflow
dotnet build && dotnet test && dotnet format --verify-no-changes

# Run API (Development)
dotnet run --project src/StreamDingo.Api/StreamDingo.Api.csproj

# Quick project structure check
dotnet sln list
```

### Solution Structure (dotnet sln list output)
```
Project(s)
----------
src/StreamDingo.Api/StreamDingo.Api.csproj
src/StreamDingo.Core/StreamDingo.Core.csproj
tests/StreamDingo.Tests/StreamDingo.Tests.csproj
```

### Repository Root Structure (ls -la output)
```
total 40
drwxr-xr-x 5 runner docker 4096 . 
drwxr-xr-x 3 runner docker 4096 ..
drwxr-xr-x 7 runner docker 4096 .git
-rw-r--r-- 1 runner docker 7370 .gitignore
-rw-r--r-- 1 runner docker 1067 LICENSE
-rw-r--r-- 1 runner docker   13 README.md
-rw-r--r-- 1 runner docker 2600 StreamDingo.sln
drwxr-xr-x 4 runner docker 4096 src
drwxr-xr-x 3 runner docker 4096 tests
```

## Important Notes

- **Code Formatting**: This repository enforces strict code formatting. Always run `dotnet format` before committing
- **Port Configuration**: The API runs on port 5155 by default in development
- **Swagger**: Development builds include Swagger UI for API documentation and testing
- **Testing**: All business logic should have corresponding unit tests in the Tests project
- **Architecture**: Follow the existing pattern of keeping business logic in Core and presentation logic in Api
- **Dependencies**: When adding new NuGet packages, add them to the appropriate project and update project references as needed

## Troubleshooting

- **Build Issues**: Run `dotnet clean && dotnet restore && dotnet build` for a fresh build
- **Test Failures**: Check that all project references are correct and dependencies are restored
- **Formatting Issues**: Run `dotnet format` to fix, then `dotnet format --verify-no-changes` to confirm
- **API Not Starting**: Verify no other process is using port 5155, check for build errors first
- **Missing Dependencies**: Run `dotnet restore` to ensure all NuGet packages are available