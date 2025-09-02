#!/bin/bash

# Build and serve documentation locally
# Usage: ./scripts/build-docs.sh [serve]

set -e

echo "🏗️  Building StreamDingo project..."
export PATH="$HOME/.dotnet:$PATH"
dotnet build src/StreamDingo/StreamDingo.csproj

echo "📚 Generating API documentation..."
python3 scripts/generate_api_docs.py src/StreamDingo/bin/Debug/net9.0/StreamDingo.xml docs/api/generated

echo "🔗 Validating documentation links..."
python3 scripts/validate_links.py docs/ --skip-external

echo "📖 Building documentation..."
mkdocs build --clean

if [ "$1" = "serve" ]; then
    echo "🚀 Starting documentation server..."
    echo "Documentation will be available at http://127.0.0.1:8000"
    mkdocs serve
else
    echo "✅ Documentation built successfully!"
    echo "📁 Static files are in the 'site/' directory"
    echo "🚀 Run './scripts/build-docs.sh serve' to start a local server"
fi