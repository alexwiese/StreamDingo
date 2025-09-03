# Release Management

StreamDingo follows a structured release management process to ensure high-quality, reliable releases.

## Semantic Versioning

StreamDingo adheres to [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html):

- **MAJOR** version (`X.0.0`): Incompatible API changes or breaking changes
- **MINOR** version (`0.X.0`): New features that are backwards compatible
- **PATCH** version (`0.0.X`): Backwards compatible bug fixes

### Version Strategy

#### Major Releases (X.0.0)
- Breaking API changes
- Significant architectural changes
- Removal of deprecated features
- Major dependency updates that break compatibility

Examples:
- Changing public interface signatures
- Removing public classes or methods
- Changing behavior that existing code depends on

#### Minor Releases (0.X.0)
- New features and functionality
- Performance improvements
- New configuration options
- Deprecation warnings (but not removal)
- Non-breaking dependency updates

Examples:
- Adding new event store implementations
- New snapshot strategies
- Additional configuration options
- New utility methods

#### Patch Releases (0.0.X)
- Bug fixes
- Security patches
- Documentation improvements
- Internal refactoring that doesn't affect public API

Examples:
- Fixing memory leaks
- Correcting edge case behaviors
- Security vulnerability patches
- Performance optimizations

## Release Process

### 1. Pre-release Checklist
- [ ] All CI/CD checks pass
- [ ] Code coverage meets minimum threshold (80%)
- [ ] Security scans show no critical issues
- [ ] Performance benchmarks show no regressions
- [ ] Documentation is up to date
- [ ] Breaking changes are documented

### 2. Version Preparation
1. Update version numbers in:
   - `src/StreamDingo/StreamDingo.csproj`
   - Package metadata
2. Update `CHANGELOG.md` with release notes
3. Run final tests and validation

### 3. Release Creation
1. Create release branch: `release/vX.Y.Z`
2. Final testing and validation
3. Create and push git tag: `git tag vX.Y.Z`
4. GitHub Actions automatically:
   - Builds and tests the release
   - Creates NuGet packages
   - Publishes to NuGet.org
   - Creates GitHub release with notes

### 4. Post-release
- [ ] Verify NuGet package availability
- [ ] Update documentation site
- [ ] Announce release on social media/forums
- [ ] Monitor for issues

## Release Channels

### Stable Releases
- Published to NuGet.org
- Full documentation and support
- Recommended for production use

### Pre-release Versions
- Alpha: Early development versions (`1.0.0-alpha.1`)
- Beta: Feature-complete but may have bugs (`1.0.0-beta.1`)
- Release Candidate: Near-final versions (`1.0.0-rc.1`)

### Development Builds
- Built from `main` branch
- Available as GitHub Actions artifacts
- Not suitable for production

## Compatibility Promise

### API Compatibility
- **Major versions**: No compatibility guarantees
- **Minor versions**: Backwards compatible
- **Patch versions**: Fully compatible

### Runtime Compatibility
- .NET version support follows Microsoft's support lifecycle
- Breaking .NET changes may trigger major version increment

### Data Compatibility
- Event store formats maintain backwards compatibility within major versions
- Migration guides provided for breaking changes
- Snapshot formats versioned independently

## Deprecation Policy

### Deprecation Timeline
1. **Deprecation Notice**: Feature marked as deprecated with warning
2. **Grace Period**: Minimum one major version before removal
3. **Removal**: Feature removed in subsequent major version

### Deprecation Communication
- API documentation updated with deprecation notices
- Compiler warnings for deprecated APIs
- Migration guides provided
- Advance notice in release notes

## Support Policy

### Long Term Support (LTS)
- Major versions receive 18 months of support
- Security patches for 24 months
- Critical bug fixes for 12 months

### Current Support Matrix

| Version | Status | Release Date | End of Life |
|---------|--------|--------------|-------------|
| 1.x     | Active | TBD          | TBD         |

### Security Updates
- Critical security issues: Immediate patch release
- High severity: Within 7 days  
- Medium/Low severity: Next scheduled release

## Changelog Format

All releases documented in `CHANGELOG.md` following [Keep a Changelog](https://keepachangelog.com/) format:

```markdown
## [1.0.0] - 2025-01-01

### Added
- New feature descriptions

### Changed  
- Modifications to existing features

### Deprecated
- Features marked for removal

### Removed
- Features removed in this version

### Fixed
- Bug fixes

### Security
- Security-related changes
```

## Release Automation

### GitHub Actions Workflows
- **CI/CD**: Automated testing and validation
- **Release**: Automatic package creation and publishing
- **Security**: Automated vulnerability scanning
- **Dependencies**: Automated dependency updates

### Quality Gates
All releases must pass:
- ✅ All unit tests (100% pass rate)
- ✅ Integration tests (100% pass rate)  
- ✅ Code coverage (≥80%)
- ✅ Performance benchmarks (no regression)
- ✅ Security scans (no critical issues)
- ✅ Static analysis (no errors)

This release management process ensures StreamDingo maintains high quality and reliability standards while providing clear communication to users about changes and compatibility.