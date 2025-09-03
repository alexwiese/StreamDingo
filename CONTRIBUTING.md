# Contributing to StreamDingo

Thank you for your interest in contributing to StreamDingo! This document provides guidelines for contributing to this .NET event sourcing library.

## Quick Start

1. **Fork the repository** on GitHub
2. **Clone your fork** locally
3. **Create a feature branch** from main
4. **Set up development environment** (see [Development Setup](#development-setup))
5. **Make your changes** following our [coding standards](#coding-standards)
6. **Add tests** for new functionality
7. **Update documentation** if needed
8. **Submit a pull request**

## Development Setup

### Prerequisites

- .NET 9.0 SDK (9.0.304 or later)
- Git
- Your favorite IDE (Visual Studio 2022, VS Code, or Rider)

### Getting Started

```bash
# Clone your fork
git clone https://github.com/your-username/StreamDingo.git
cd StreamDingo

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test

# Run benchmarks (optional)
dotnet run --project benchmarks/StreamDingo.Benchmarks
```

## How to Contribute

### Reporting Bugs

Before creating a bug report:
- Check if the bug has already been reported
- Use the latest version to verify the bug still exists
- Collect relevant information (OS, .NET version, error messages)

Create a detailed bug report including:
- Clear, descriptive title
- Steps to reproduce
- Expected vs actual behavior
- Minimal code example
- Environment details

### Suggesting Features

Feature suggestions are welcome! Please:
- Check if the feature has been suggested before
- Explain the use case and benefits
- Consider if it fits the library's scope
- Provide examples of how it would work

### Code Contributions

#### Coding Standards

- Follow .NET naming conventions and coding standards
- Use meaningful variable and method names
- Add XML documentation for all public APIs
- Keep methods focused and concise (single responsibility)
- Use immutable data structures where possible
- Follow existing code patterns and architecture

#### Code Quality

- **Test Coverage**: All new functionality must have tests
- **Documentation**: Update docs for new features or breaking changes
- **Performance**: Consider performance implications of changes
- **Memory**: Minimize allocations in hot paths
- **Thread Safety**: Ensure thread-safe implementations where needed

#### Commit Guidelines

- Use clear, descriptive commit messages
- Reference issue numbers when applicable
- Keep commits focused and atomic
- Write commit messages in imperative mood

Example commit message:
```
Add IEventStore interface with basic operations

- Define core event store abstraction
- Include async methods for event operations  
- Add comprehensive XML documentation
- Fixes #123
```

## Code Review Process

1. **Automated Checks**: All PRs must pass CI/CD checks
   - Build succeeds
   - All tests pass
   - Code coverage maintained
   - Benchmarks show no significant regression

2. **Manual Review**: Maintainers will review for:
   - Code quality and standards
   - Architecture alignment
   - Documentation completeness
   - Test coverage

3. **Feedback**: Address review comments promptly
4. **Approval**: Requires approval from at least one maintainer

## Testing Requirements

### Unit Tests
- Test all public APIs
- Cover edge cases and error conditions
- Use descriptive test names
- Follow Arrange-Act-Assert pattern

### Integration Tests
- Test real-world scenarios
- Verify component interactions
- Include performance-sensitive paths

### Benchmark Tests
- For performance-critical changes
- Compare before/after performance
- Include memory allocation metrics

## Documentation

### API Documentation
- All public types and members need XML documentation
- Include usage examples in documentation
- Document parameters, return values, and exceptions

### User Documentation
- Update getting started guides for new features
- Add code examples and tutorials
- Keep documentation current with code changes

## Community Guidelines

Please follow our [Code of Conduct](CODE_OF_CONDUCT.md) in all interactions.

### Communication
- Be respectful and inclusive
- Ask questions if something is unclear
- Help others learn and grow
- Focus on constructive feedback

### Recognition
- Contributors are recognized in release notes
- Significant contributions may result in collaborator status
- Community involvement is valued and appreciated

## Release Process

StreamDingo follows [semantic versioning](https://semver.org/):
- **Major** (x.0.0): Breaking changes
- **Minor** (0.x.0): New features, backwards compatible
- **Patch** (0.0.x): Bug fixes, backwards compatible

## Getting Help

- **Documentation**: Check the [official docs](https://alexwiese.github.io/StreamDingo/)
- **Issues**: Search existing GitHub issues
- **Discussions**: Use GitHub Discussions for questions
- **Discord**: Join our community Discord server (coming soon)

## License

By contributing to StreamDingo, you agree that your contributions will be licensed under the same license as the project (see [LICENSE](LICENSE)).

---

Thank you for contributing to StreamDingo! 🚀