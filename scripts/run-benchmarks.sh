#!/bin/bash

# StreamDingo Benchmark Runner Script
# Automated benchmark execution with result handling

set -e

# Configuration
BENCHMARK_PROJECT="benchmarks/StreamDingo.Benchmarks/StreamDingo.Benchmarks.csproj"
RESULTS_DIR="benchmark-results"
ARTIFACTS_DIR="BenchmarkDotNet.Artifacts"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BASELINE_FILE="baseline-results.json"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')] $1${NC}"
}

warn() {
    echo -e "${YELLOW}[WARNING] $1${NC}"
}

error() {
    echo -e "${RED}[ERROR] $1${NC}"
}

success() {
    echo -e "${GREEN}[SUCCESS] $1${NC}"
}

# Function to show usage
show_usage() {
    echo "StreamDingo Benchmark Runner"
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -h, --help          Show this help message"
    echo "  -f, --filter FILTER Run benchmarks matching the filter pattern"
    echo "  -c, --category CAT  Run benchmarks in specific category"
    echo "  -b, --baseline      Save results as baseline for future comparisons"
    echo "  -r, --regression    Compare results against baseline (performance regression detection)"
    echo "  -q, --quick         Run quick benchmarks (shorter iterations)"
    echo "  -a, --all           Run all benchmarks (default)"
    echo "  --dry               Run dry benchmarks (single execution)"
    echo "  --memory            Include memory diagnostics"
    echo "  --threading         Include threading diagnostics"
    echo "  --export FORMAT     Export format: markdown, html, json, csv (default: all)"
    echo ""
    echo "Examples:"
    echo "  $0 --all                           # Run all benchmarks"
    echo "  $0 --category Hash                 # Run only hash benchmarks"
    echo "  $0 --filter \"*Append*\"             # Run benchmarks with 'Append' in name"
    echo "  $0 --baseline                      # Save results as baseline"
    echo "  $0 --regression                    # Check for performance regressions"
    echo "  $0 --quick --memory                # Quick run with memory diagnostics"
}

# Check if .NET is available
check_dotnet() {
    if ! command -v dotnet &> /dev/null; then
        error "dotnet CLI is not available. Please install .NET 9 SDK."
        exit 1
    fi
    
    local version=$(dotnet --version)
    log "Using .NET SDK version: $version"
    
    if [[ ! "$version" == 9.* ]]; then
        warn "Expected .NET 9.x, found $version. Some features might not work correctly."
    fi
}

# Create results directory
prepare_directories() {
    log "Preparing directories..."
    
    mkdir -p "$RESULTS_DIR"
    
    if [ -d "$ARTIFACTS_DIR" ]; then
        log "Cleaning previous artifacts..."
        rm -rf "$ARTIFACTS_DIR"
    fi
}

# Build the benchmark project
build_project() {
    log "Building benchmark project..."
    
    if ! dotnet build "$BENCHMARK_PROJECT" --configuration Release --no-restore; then
        error "Failed to build benchmark project"
        exit 1
    fi
    
    success "Benchmark project built successfully"
}

# Run benchmarks with specified options
run_benchmarks() {
    local args=()
    local output_file="${RESULTS_DIR}/benchmark-${TIMESTAMP}.json"
    
    # Build dotnet run arguments
    args+=(--project "$BENCHMARK_PROJECT")
    args+=(--configuration Release)
    args+=(--no-build)
    args+=(--verbosity minimal)
    args+=(--)
    
    # Add benchmark-specific arguments
    if [ -n "$FILTER" ]; then
        args+=(--filter "$FILTER")
        log "Running benchmarks matching filter: $FILTER"
    elif [ -n "$CATEGORY" ]; then
        args+=(--anyCategories "$CATEGORY")
        log "Running benchmarks in category: $CATEGORY"
    else
        log "Running all benchmarks"
    fi
    
    if [ "$DRY_RUN" = true ]; then
        args+=(--job Dry)
        log "Running dry benchmarks (single execution)"
    elif [ "$QUICK_RUN" = true ]; then
        args+=(--job Short)
        log "Running quick benchmarks"
    fi
    
    if [ "$MEMORY_DIAGNOSTICS" = true ]; then
        args+=(--memory)
        log "Including memory diagnostics"
    fi
    
    if [ "$THREADING_DIAGNOSTICS" = true ]; then
        args+=(--threading)
        log "Including threading diagnostics"
    fi
    
    # Add exporters
    case "$EXPORT_FORMAT" in
        "markdown") args+=(--exporters GitHub) ;;
        "html") args+=(--exporters HTML) ;;
        "json") args+=(--exporters JSON) ;;
        "csv") args+=(--exporters CSV) ;;
        *) args+=(--exporters GitHub HTML JSON CSV) ;;
    esac
    
    # Add artifacts directory
    args+=(--artifacts "$ARTIFACTS_DIR")
    
    log "Starting benchmark execution..."
    log "Command: dotnet run ${args[*]}"
    
    if dotnet run "${args[@]}"; then
        success "Benchmarks completed successfully"
        
        # Move JSON results to our results directory if they exist
        if [ -f "$ARTIFACTS_DIR/results/StreamDingo.Benchmarks-report.json" ]; then
            cp "$ARTIFACTS_DIR/results/StreamDingo.Benchmarks-report.json" "$output_file"
            log "Results saved to: $output_file"
        fi
        
        return 0
    else
        error "Benchmark execution failed"
        return 1
    fi
}

# Save baseline results
save_baseline() {
    local latest_result=$(ls -t "$RESULTS_DIR"/benchmark-*.json 2>/dev/null | head -n1)
    
    if [ -n "$latest_result" ]; then
        cp "$latest_result" "$RESULTS_DIR/$BASELINE_FILE"
        success "Baseline saved: $RESULTS_DIR/$BASELINE_FILE"
    else
        error "No benchmark results found to save as baseline"
        return 1
    fi
}

# Compare against baseline (regression detection)
check_regression() {
    if [ ! -f "$RESULTS_DIR/$BASELINE_FILE" ]; then
        warn "No baseline file found. Run with --baseline first to create baseline."
        return 1
    fi
    
    local latest_result=$(ls -t "$RESULTS_DIR"/benchmark-*.json 2>/dev/null | head -n1)
    
    if [ -z "$latest_result" ]; then
        error "No recent benchmark results found for comparison"
        return 1
    fi
    
    log "Comparing against baseline..."
    log "Baseline: $RESULTS_DIR/$BASELINE_FILE"
    log "Current:  $latest_result"
    
    # TODO: Implement actual regression analysis
    # This would require parsing JSON and comparing performance metrics
    # For now, just show the files exist
    
    success "Regression check completed. Manual comparison required."
    echo "Compare these files manually or use BenchmarkDotNet's comparison tools:"
    echo "  Baseline: $RESULTS_DIR/$BASELINE_FILE"
    echo "  Current:  $latest_result"
}

# Generate summary report
generate_summary() {
    log "Generating summary report..."
    
    local summary_file="$RESULTS_DIR/summary-${TIMESTAMP}.md"
    
    cat > "$summary_file" << EOF
# StreamDingo Benchmark Summary

**Generated:** $(date)
**Timestamp:** $TIMESTAMP

## Configuration
- .NET Version: $(dotnet --version)
- OS: $(uname -s) $(uname -r)
- Architecture: $(uname -m)

## Results Location
- Results Directory: $RESULTS_DIR
- Artifacts Directory: $ARTIFACTS_DIR

## Benchmark Categories
EOF

    # List available benchmark categories if artifacts exist
    if [ -d "$ARTIFACTS_DIR" ]; then
        echo "- See detailed results in $ARTIFACTS_DIR/results/" >> "$summary_file"
    fi

    success "Summary report generated: $summary_file"
}

# Parse command line arguments
FILTER=""
CATEGORY=""
SAVE_BASELINE=false
CHECK_REGRESSION=false
QUICK_RUN=false
DRY_RUN=false
MEMORY_DIAGNOSTICS=false
THREADING_DIAGNOSTICS=false
EXPORT_FORMAT="all"
RUN_ALL=true

while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_usage
            exit 0
            ;;
        -f|--filter)
            FILTER="$2"
            RUN_ALL=false
            shift 2
            ;;
        -c|--category)
            CATEGORY="$2"
            RUN_ALL=false
            shift 2
            ;;
        -b|--baseline)
            SAVE_BASELINE=true
            shift
            ;;
        -r|--regression)
            CHECK_REGRESSION=true
            shift
            ;;
        -q|--quick)
            QUICK_RUN=true
            shift
            ;;
        -a|--all)
            RUN_ALL=true
            shift
            ;;
        --dry)
            DRY_RUN=true
            shift
            ;;
        --memory)
            MEMORY_DIAGNOSTICS=true
            shift
            ;;
        --threading)
            THREADING_DIAGNOSTICS=true
            shift
            ;;
        --export)
            EXPORT_FORMAT="$2"
            shift 2
            ;;
        *)
            error "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Main execution
main() {
    log "StreamDingo Benchmark Runner Starting..."
    
    check_dotnet
    prepare_directories
    
    # Restore dependencies
    log "Restoring dependencies..."
    dotnet restore
    
    build_project
    
    if run_benchmarks; then
        generate_summary
        
        if [ "$SAVE_BASELINE" = true ]; then
            save_baseline
        fi
        
        if [ "$CHECK_REGRESSION" = true ]; then
            check_regression
        fi
        
        success "Benchmark run completed successfully!"
        
        # Show results location
        echo ""
        echo "Results available at:"
        echo "  - JSON: $RESULTS_DIR/"
        echo "  - Artifacts: $ARTIFACTS_DIR/"
        
        if [ -d "$ARTIFACTS_DIR/results" ]; then
            echo "  - Reports: $ARTIFACTS_DIR/results/"
        fi
        
    else
        error "Benchmark run failed!"
        exit 1
    fi
}

# Run main function
main "$@"