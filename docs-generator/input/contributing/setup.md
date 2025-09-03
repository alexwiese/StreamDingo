---
Layout: _Layout
Title: setup
---
# Development Setup

Set up your development environment for contributing to StreamDingo.

## Prerequisites

- .NET 9.0 SDK or later
- Git
- Your preferred IDE (Visual Studio 2022, VS Code, or JetBrains Rider)

## Getting Started

1. **Clone the repository**
   ```bash
   git clone https://github.com/alexwiese/StreamDingo.git
   cd StreamDingo
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

## Development Workflow

1. Create a feature branch
2. Make your changes
3. Run tests and ensure they pass
4. Submit a pull request

## Project Structure

- `src/StreamDingo/` - Main library code
- `tests/StreamDingo.Tests/` - Unit and integration tests  
- `docs/` - Documentation source
- `examples/` - Usage examples and samples

## Code Style

- Follow .NET coding conventions
- Use EditorConfig settings (already configured)
- Run `dotnet format` before committing
- Add XML documentation comments for public APIs

## Testing

- Write unit tests for new functionality
- Ensure existing tests continue to pass
- Add integration tests for complex scenarios
- Maintain test coverage above 90%

More detailed setup instructions coming soon.
