<#
.SYNOPSIS
    Comprehensive CI/CD test script for Windows PowerShell
#>
param(
    [string]$ApiKey = ''
)

# Color helper using Write-Host -ForegroundColor
function Write-Info($msg) { Write-Host $msg -ForegroundColor Cyan }
function Write-Success($msg) { Write-Host "[OK] $msg" -ForegroundColor Green }
function Write-WarningMsg($msg) { Write-Host "[WARN] $msg" -ForegroundColor Yellow }
function Write-ErrorMsg($msg) { Write-Host "[ERROR] $msg" -ForegroundColor Red }

function Write-Header($text) {
    Write-Host '' # Add a blank line for spacing
    Write-Info "=== $text ==="
}

# Docker command with Windows/WSL fallback
function Invoke-Docker {
    # Uses automatic $args array for flexibility across PS versions
    if (Get-Command docker -ErrorAction SilentlyContinue) {
        docker @args
    } elseif (Test-Path "C:\Program Files\Docker\Docker\resources\bin\docker.exe") {
        & "C:\Program Files\Docker\Docker\resources\bin\docker.exe" @args
    } else {
        Write-ErrorMsg "ERROR: Docker not found. Please install Docker Desktop and enable WSL integration."
        exit 1
    }
}

# Create Logs directory if it doesn't exist
$LogDir = Join-Path $PSScriptRoot "../Logs"
if (-not (Test-Path $LogDir)) { New-Item -ItemType Directory -Path $LogDir | Out-Null }
$LogFile = Join-Path $LogDir "test-ci-cd-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"
# Logging is now handled explicitly for each command to provide real-time feedback.
Write-Host "=== Script started at $(Get-Date) ==="
"=== Script started at $(Get-Date) ===" | Out-File -FilePath $LogFile -Append
Write-Host "Logging to: $LogFile"

# Load API key from (1) parameter, (2) env var, (3) .atera_apikey file
# This logic is flattened to prevent parser errors with nested braces.
$keyFile = Join-Path $PSScriptRoot '..\.atera_apikey'
if (-not $ApiKey) {
    $ApiKey = $env:Atera__ApiKey
}
if (-not $ApiKey -and (Test-Path $keyFile)) {
    $ApiKey = (Get-Content $keyFile -Raw).Trim()
}

# Set environment variable if key was found, otherwise warn
if ($ApiKey) {
    $env:Atera__ApiKey = $ApiKey
    Write-Success "API key has been set for this session."
} else {
    Write-WarningMsg "API key not set; integration tests may fail."
}

# Helper to check command exit codes and abort on failure
function Assert-LastCommandSucceeded {
    if ($LASTEXITCODE -ne 0) {
        Write-ErrorMsg "Previous command failed with exit code $LASTEXITCODE. Aborting."
        exit $LASTEXITCODE
    }
}

Write-Header '1. Windows Debug Build and Test'
& dotnet test "AteraMcp.sln" -c Debug --no-restore --verbosity normal *>&1 | Tee-Object -FilePath $LogFile -Append
Assert-LastCommandSucceeded

Write-Header '2. Windows Release Build and Test'
& dotnet test "AteraMcp.sln" -c Release --no-restore --verbosity normal *>&1 | Tee-Object -FilePath $LogFile -Append
Assert-LastCommandSucceeded

Write-Header '3. Docker Build and Test'
Invoke-Docker build -t atera-mcp-server . *>&1 | Tee-Object -FilePath $LogFile -Append
Assert-LastCommandSucceeded

Invoke-Docker run --rm -e Atera__ApiKey=$ApiKey atera-mcp-server dotnet test AteraMcp.IntegrationTests.dll --no-build --verbosity normal *>&1 | Tee-Object -FilePath $LogFile -Append
Assert-LastCommandSucceeded

Write-Host '' # Add a blank line for spacing
Write-Success "All Windows tests completed successfully"

Write-Host "=== Script completed at $(Get-Date) ==="
"=== Script completed at $(Get-Date) ===" | Out-File -FilePath $LogFile -Append
