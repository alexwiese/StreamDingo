# Documentation Scripts

This directory contains scripts for generating and maintaining StreamDingo's documentation.

## Scripts Overview

### build-docs.sh
**Purpose**: Complete documentation build and validation pipeline
**Usage**: 
```bash
# Build documentation
./scripts/build-docs.sh

# Build and serve documentation locally
./scripts/build-docs.sh serve
```

**What it does**:
1. Builds the .NET project to generate XML documentation
2. Generates API documentation from XML comments
3. Validates all documentation links
4. Builds the MkDocs site
5. Optionally serves the documentation locally

### generate_api_docs.py
**Purpose**: Generate markdown API documentation from .NET XML documentation files
**Usage**: 
```bash
python3 scripts/generate_api_docs.py <xml_file> <output_dir>
```

**Example**:
```bash
python3 scripts/generate_api_docs.py src/StreamDingo/bin/Debug/net9.0/StreamDingo.xml docs/api/generated
```

**Features**:
- Parses .NET XML documentation format
- Groups documentation by class
- Generates clean markdown files
- Supports methods, properties, fields, and type documentation
- Creates navigation index automatically

### validate_links.py
**Purpose**: Validate all links in documentation markdown files
**Usage**:
```bash
# Validate all links including external ones
python3 scripts/validate_links.py docs/

# Skip external link validation (faster)
python3 scripts/validate_links.py docs/ --skip-external
```

**Features**:
- Validates local file links
- Validates external HTTP/HTTPS links
- Supports markdown link formats: `[text](url)` and `[ref]: url`
- Handles relative and absolute paths
- Provides detailed error reporting
- Rate-limits external requests to be server-friendly

## Documentation Workflow

The documentation workflow is integrated into GitHub Actions (`.github/workflows/docs.yml`):

1. **Build Phase**:
   - Install Python dependencies (MkDocs + plugins)
   - Set up .NET SDK
   - Build StreamDingo project
   - Generate API documentation
   - Build MkDocs site
   - Validate documentation links

2. **Deploy Phase**:
   - Upload site to GitHub Pages
   - Deploy to `https://alexwiese.github.io/StreamDingo`

## Local Development

For local documentation development:

1. **Install Prerequisites**:
   ```bash
   # Install .NET 9.0 SDK
   # Install Python 3 with pip
   
   # Install Python dependencies
   pip install mkdocs mkdocs-material mkdocs-mermaid2-plugin 
   pip install mkdocs-git-revision-date-localized-plugin mkdocs-minify-plugin
   pip install requests beautifulsoup4
   ```

2. **Generate Documentation**:
   ```bash
   ./scripts/build-docs.sh serve
   ```

3. **Manual Steps** (if needed):
   ```bash
   # Build project
   dotnet build src/StreamDingo/StreamDingo.csproj
   
   # Generate API docs
   python3 scripts/generate_api_docs.py src/StreamDingo/bin/Debug/net9.0/StreamDingo.xml docs/api/generated
   
   # Validate links
   python3 scripts/validate_links.py docs/ --skip-external
   
   # Build and serve
   mkdocs serve
   ```

## Documentation Structure

```
docs/
├── index.md                    # Home page
├── getting-started/            # Installation and quick start
├── guide/                      # User guides
├── api/                        # API documentation
│   ├── generated/              # Auto-generated from XML comments
│   └── *.md                    # Hand-written API guides
├── advanced/                   # Advanced topics
└── contributing/               # Contributor documentation
```

The generated API documentation in `docs/api/generated/` should not be manually edited as it will be overwritten during the build process.

## Adding New Documentation

1. **Manual Pages**: Add `.md` files to appropriate directories
2. **Navigation**: Update `mkdocs.yml` nav section
3. **API Documentation**: Add XML comments to source code; API docs will be auto-generated
4. **Validation**: Run link validation before committing
5. **Testing**: Use `./scripts/build-docs.sh serve` to test locally

## Troubleshooting

**Common Issues**:

- **Build fails**: Ensure .NET 9.0 SDK is installed and project builds
- **API docs empty**: Check that XML documentation is enabled in `.csproj`
- **Links broken**: Run link validation to identify issues
- **MkDocs errors**: Check that all Python dependencies are installed

**Debugging**:
- Use `mkdocs build --verbose` for detailed build output
- Check GitHub Actions logs for deployment issues
- Verify XML documentation file exists after building project