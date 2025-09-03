# StreamDingo Documentation Generator

This directory contains the Statiq Docs-based documentation generator for StreamDingo.

## Overview

This replaces the previous mkdocs-based documentation system with a .NET-native solution using Statiq Docs.

## Features

- **Statiq Docs**: Modern .NET static site generator
- **Integrated API Docs**: Automatically generates API documentation from .NET XML docs
- **Theme**: CleanBlog theme with responsive design
- **Search**: Lunr.js powered search functionality
- **GitHub Integration**: Automatic edit links and repository integration

## Usage

### Build Documentation

```bash
# From repository root
./scripts/build-docs.sh
```

### Serve Locally

```bash
# From repository root  
./scripts/build-docs.sh serve
```

### Manual Build

```bash
# From docs-generator directory
dotnet run
```

## Structure

- `Program.cs` - Main Statiq Docs configuration
- `appsettings.yml` - Site configuration (title, theme, etc.)
- `input/` - Documentation source files (copied from `/docs/`)
- `output/` - Generated static site (ignored in git)
- `cache/` - Statiq build cache (ignored in git)

## Configuration

Key settings in `appsettings.yml`:

- `SiteName`: Site title
- `Theme`: CleanBlog (can be changed to other Statiq themes)
- `SearchProvider`: Lunr for client-side search
- `GitHub`: Repository integration for edit links
- `ProjectFiles`: Points to StreamDingo.csproj for API doc generation

## Migration from mkdocs

The migration involved:

1. **Created Statiq Docs project** with equivalent functionality
2. **Fixed YAML frontmatter issues** by adding empty frontmatter to files starting with blockquotes
3. **Integrated API documentation generation** directly into Statiq pipeline
4. **Updated GitHub Actions workflow** to use .NET instead of Python
5. **Maintained directory structure** and navigation

## Deployment

The documentation is automatically deployed to GitHub Pages via `.github/workflows/docs.yml` when changes are pushed to main branch or docs-related files are modified.

## Theme Customization

To customize the theme:

1. Change `Theme` in `appsettings.yml` (options: CleanBlog, Phantom, etc.)
2. Add custom CSS/JS by creating theme override files
3. Modify the Program.cs for advanced customizations

## Dependencies

- **.NET 9.0 SDK**: Required for building
- **Statiq.Docs**: Static site generator
- **Statiq.CodeAnalysis**: For API documentation generation

## Troubleshooting

- **Build failures**: Ensure .NET 9.0 SDK is installed
- **Missing API docs**: Verify StreamDingo project builds and generates XML docs
- **Theme issues**: Check theme name in appsettings.yml
- **Link validation warnings**: Expected for .md internal links in source files