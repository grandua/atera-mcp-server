# PowerShell build script for cross-platform CI/CD
$ErrorActionPreference = "Stop"
$transcriptPath = Join-Path $PSScriptRoot "..\build-script.log"
Start-Transcript -Path $transcriptPath -Append

# Configuration
$CONFIG = if ($env:BUILD_CONFIGURATION) { $env:BUILD_CONFIGURATION } else { "Release" }

Write-Host "=== PowerShell Build Script Started (Config: $CONFIG) ===" -ForegroundColor Cyan

# Workspace detection
$WORKSPACE = if ($env:GITHUB_WORKSPACE) { $env:GITHUB_WORKSPACE } else { $PSScriptRoot | Split-Path -Parent | Split-Path -Parent }

# API key handling: fallback to .atera_apikey file
if (-not $env:Atera__ApiKey) {
    $keyFile = Join-Path $WORKSPACE ".atera_apikey"
    if (Test-Path $keyFile) {
        $env:Atera__ApiKey = (Get-Content $keyFile -Raw).Trim()
        Write-Host "Loaded Atera__ApiKey from .atera_apikey file" -ForegroundColor Green
    } else {
        Write-Host "Warning: Atera__ApiKey not set and .atera_apikey file not found. Integration tests may fail." -ForegroundColor Yellow
    }
}

# Solution verification
try {
    Set-Location $WORKSPACE -ErrorAction Stop
    if (-not (Test-Path "AteraMcp.sln")) {
        throw "Solution file not found in $WORKSPACE"
    }
    Write-Host "Solution verified in $(Get-Location)" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: $_" -ForegroundColor Red
    Stop-Transcript
    exit 1
}

# Build pipeline
try {
    # Restore
    Write-Host "Restoring dependencies..." -ForegroundColor Yellow
    dotnet restore
    
    # Build
    Write-Host "Building solution ($CONFIG configuration)..." -ForegroundColor Yellow
    dotnet build -c $CONFIG --no-restore
    
    # Test
    Write-Host "Running tests ($CONFIG configuration)..." -ForegroundColor Yellow
    dotnet test -c $CONFIG --no-build --no-restore
}
catch {
    Write-Host "BUILD FAILED: $_" -ForegroundColor Red
    Stop-Transcript
    exit 1
}

Write-Host "Build completed successfully" -ForegroundColor Green
Stop-Transcript
