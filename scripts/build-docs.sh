#!/bin/bash

# Build and serve documentation locally with Statiq Docs
# Usage: ./scripts/build-docs.sh [serve]

set -e

echo "🏗️  Building StreamDingo project..."
export PATH="$HOME/.dotnet:$PATH"
dotnet build src/StreamDingo/StreamDingo.csproj

echo "📖 Building documentation with Statiq..."
cd docs-generator
dotnet run

if [ "$1" = "serve" ]; then
    echo "🚀 Starting documentation server..."
    echo "Documentation will be available at http://127.0.0.1:5080"
    dotnet run -- serve
else
    echo "✅ Documentation built successfully!"
    echo "📁 Static files are in the 'docs-generator/output/' directory"
    echo "🚀 Run './scripts/build-docs.sh serve' to start a local server"
fi