# StreamDingo Benchmark Runner Script (PowerShell)
# Automated benchmark execution with result handling

param(
    [string]$Filter = "",
    [string]$Category = "",
    [switch]$Baseline,
    [switch]$Regression, 
    [switch]$Quick,
    [switch]$All = $true,
    [switch]$Dry,
    [switch]$Memory,
    [switch]$Threading,
    [string]$Export = "all",
    [switch]$Help
)

# Configuration
$BenchmarkProject = "benchmarks/StreamDingo.Benchmarks/StreamDingo.Benchmarks.csproj"
$ResultsDir = "benchmark-results"
$ArtifactsDir = "BenchmarkDotNet.Artifacts"
$Timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$BaselineFile = "baseline-results.json"

function Write-Log {
    param([string]$Message)
    Write-Host "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

function Show-Usage {
    Write-Host "StreamDingo Benchmark Runner (PowerShell)"
    Write-Host "Usage: .\scripts\run-benchmarks.ps1 [OPTIONS]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Help                   Show this help message"
    Write-Host "  -Filter <FILTER>        Run benchmarks matching the filter pattern"
    Write-Host "  -Category <CAT>         Run benchmarks in specific category"
    Write-Host "  -Baseline              Save results as baseline for future comparisons"
    Write-Host "  -Regression            Compare results against baseline"
    Write-Host "  -Quick                 Run quick benchmarks (shorter iterations)"
    Write-Host "  -All                   Run all benchmarks (default)"
    Write-Host "  -Dry                   Run dry benchmarks (single execution)"
    Write-Host "  -Memory                Include memory diagnostics"
    Write-Host "  -Threading             Include threading diagnostics"
    Write-Host "  -Export <FORMAT>       Export format: markdown, html, json, csv (default: all)"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\scripts\run-benchmarks.ps1 -All"
    Write-Host "  .\scripts\run-benchmarks.ps1 -Category Hash"
    Write-Host "  .\scripts\run-benchmarks.ps1 -Filter '*Append*'"
    Write-Host "  .\scripts\run-benchmarks.ps1 -Baseline"
    Write-Host "  .\scripts\run-benchmarks.ps1 -Regression"
    Write-Host "  .\scripts\run-benchmarks.ps1 -Quick -Memory"
}

function Test-DotNet {
    if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Error "dotnet CLI is not available. Please install .NET 9 SDK."
        exit 1
    }
    
    $version = dotnet --version
    Write-Log "Using .NET SDK version: $version"
    
    if (!$version.StartsWith("9.")) {
        Write-Warning "Expected .NET 9.x, found $version. Some features might not work correctly."
    }
}

function Initialize-Directories {
    Write-Log "Preparing directories..."
    
    if (!(Test-Path $ResultsDir)) {
        New-Item -ItemType Directory -Path $ResultsDir -Force | Out-Null
    }
    
    if (Test-Path $ArtifactsDir) {
        Write-Log "Cleaning previous artifacts..."
        Remove-Item -Recurse -Force $ArtifactsDir
    }
}

function Build-Project {
    Write-Log "Building benchmark project..."
    
    $buildResult = dotnet build $BenchmarkProject --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build benchmark project"
        exit 1
    }
    
    Write-Success "Benchmark project built successfully"
}

function Start-Benchmarks {
    $outputFile = "$ResultsDir/benchmark-$Timestamp.json"
    
    # Build dotnet run arguments
    $args = @(
        "run",
        "--project", $BenchmarkProject,
        "--configuration", "Release",
        "--no-build",
        "--verbosity", "minimal",
        "--"
    )
    
    # Add benchmark-specific arguments
    if ($Filter) {
        $args += @("--filter", $Filter)
        Write-Log "Running benchmarks matching filter: $Filter"
    }
    elseif ($Category) {
        $args += @("--anyCategories", $Category)
        Write-Log "Running benchmarks in category: $Category"
    }
    else {
        Write-Log "Running all benchmarks"
    }
    
    if ($Dry) {
        $args += @("--job", "Dry")
        Write-Log "Running dry benchmarks (single execution)"
    }
    elseif ($Quick) {
        $args += @("--job", "Short")
        Write-Log "Running quick benchmarks"
    }
    
    if ($Memory) {
        $args += "--memory"
        Write-Log "Including memory diagnostics"
    }
    
    if ($Threading) {
        $args += "--threading"
        Write-Log "Including threading diagnostics"
    }
    
    # Add exporters
    switch ($Export.ToLower()) {
        "markdown" { $args += @("--exporters", "GitHub") }
        "html" { $args += @("--exporters", "HTML") }
        "json" { $args += @("--exporters", "JSON") }
        "csv" { $args += @("--exporters", "CSV") }
        default { $args += @("--exporters", "GitHub", "HTML", "JSON", "CSV") }
    }
    
    # Add artifacts directory
    $args += @("--artifacts", $ArtifactsDir)
    
    Write-Log "Starting benchmark execution..."
    Write-Log "Command: dotnet $($args -join ' ')"
    
    & dotnet $args
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Benchmarks completed successfully"
        
        # Move JSON results to our results directory if they exist
        $jsonResult = Get-ChildItem "$ArtifactsDir/results" -Filter "*.json" | Select-Object -First 1
        if ($jsonResult) {
            Copy-Item $jsonResult.FullName $outputFile
            Write-Log "Results saved to: $outputFile"
        }
        
        return $true
    }
    else {
        Write-Error "Benchmark execution failed"
        return $false
    }
}

function Save-Baseline {
    $latestResult = Get-ChildItem "$ResultsDir/benchmark-*.json" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    
    if ($latestResult) {
        Copy-Item $latestResult.FullName "$ResultsDir/$BaselineFile"
        Write-Success "Baseline saved: $ResultsDir/$BaselineFile"
    }
    else {
        Write-Error "No benchmark results found to save as baseline"
        return $false
    }
}

function Test-Regression {
    if (!(Test-Path "$ResultsDir/$BaselineFile")) {
        Write-Warning "No baseline file found. Run with -Baseline first to create baseline."
        return $false
    }
    
    $latestResult = Get-ChildItem "$ResultsDir/benchmark-*.json" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    
    if (!$latestResult) {
        Write-Error "No recent benchmark results found for comparison"
        return $false
    }
    
    Write-Log "Comparing against baseline..."
    Write-Log "Baseline: $ResultsDir/$BaselineFile"
    Write-Log "Current:  $($latestResult.FullName)"
    
    # TODO: Implement actual regression analysis
    # This would require parsing JSON and comparing performance metrics
    # For now, just show the files exist
    
    Write-Success "Regression check completed. Manual comparison required."
    Write-Host "Compare these files manually or use BenchmarkDotNet's comparison tools:"
    Write-Host "  Baseline: $ResultsDir/$BaselineFile"
    Write-Host "  Current:  $($latestResult.FullName)"
}

function New-SummaryReport {
    Write-Log "Generating summary report..."
    
    $summaryFile = "$ResultsDir/summary-$Timestamp.md"
    $version = dotnet --version
    $os = "$($env:OS) $(Get-WmiObject Win32_OperatingSystem | Select-Object -ExpandProperty Version)"
    
    $summary = @"
# StreamDingo Benchmark Summary

**Generated:** $(Get-Date)
**Timestamp:** $Timestamp

## Configuration
- .NET Version: $version
- OS: $os
- Architecture: $env:PROCESSOR_ARCHITECTURE

## Results Location
- Results Directory: $ResultsDir
- Artifacts Directory: $ArtifactsDir

## Benchmark Categories
"@

    if (Test-Path $ArtifactsDir) {
        $summary += "`n- See detailed results in $ArtifactsDir/results/"
    }

    $summary | Out-File -FilePath $summaryFile -Encoding UTF8
    Write-Success "Summary report generated: $summaryFile"
}

# Main execution
function Main {
    if ($Help) {
        Show-Usage
        return
    }
    
    Write-Log "StreamDingo Benchmark Runner Starting..."
    
    Test-DotNet
    Initialize-Directories
    
    # Restore dependencies
    Write-Log "Restoring dependencies..."
    dotnet restore
    
    Build-Project
    
    if (Start-Benchmarks) {
        New-SummaryReport
        
        if ($Baseline) {
            Save-Baseline
        }
        
        if ($Regression) {
            Test-Regression
        }
        
        Write-Success "Benchmark run completed successfully!"
        
        # Show results location
        Write-Host ""
        Write-Host "Results available at:"
        Write-Host "  - JSON: $ResultsDir/"
        Write-Host "  - Artifacts: $ArtifactsDir/"
        
        if (Test-Path "$ArtifactsDir/results") {
            Write-Host "  - Reports: $ArtifactsDir/results/"
        }
    }
    else {
        Write-Error "Benchmark run failed!"
        exit 1
    }
}

# Execute main function
Main