# PowerShell build script equivalent to build.sh, with detailed output and transcript logging
$ErrorActionPreference = "Stop"
$transcriptPath = Join-Path $PSScriptRoot "..\build-script.log"
Start-Transcript -Path $transcriptPath -Append

Write-Host "=== PowerShell Build Script Started ===" -ForegroundColor Cyan

# Detect platform and set workspace path accordingly
if ($env:GITHUB_WORKSPACE) {
    $CONTAINER_WORKSPACE = $env:GITHUB_WORKSPACE
} elseif ([System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::Windows)) {
    $CONTAINER_WORKSPACE = "C:\Work\Projects\Fiverr\AteraMcpServer"
} else {
    $CONTAINER_WORKSPACE = "/mnt/c/Work/Projects/Fiverr/AteraMcpServer"
}
try {
    Write-Host "Navigating to: $CONTAINER_WORKSPACE" -ForegroundColor Yellow
    Set-Location $CONTAINER_WORKSPACE -ErrorAction Stop
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Failed to navigate to $CONTAINER_WORKSPACE" -ForegroundColor Red
    Write-Host "Error details: $_" -ForegroundColor Red
    Stop-Transcript
    exit 1
}

# Debug info
Write-Host "--- Directory Contents ---" -ForegroundColor Yellow
Get-ChildItem | Format-Table Name, LastWriteTime -AutoSize
Write-Host ""

# Verify solution exists
if (-not (Test-Path "AteraMcp.sln")) {
    Write-Host "ERROR: Solution file not found at: $(Get-Location)/AteraMcp.sln" -ForegroundColor Red
    Stop-Transcript
    exit 1
}

# Build commands with output and error handling
$commands = @(
    { dotnet restore "AteraMcp.sln" },
    { dotnet build "AteraMcp.sln" -c Release --no-restore },
    { dotnet test "AteraMcp.sln" -c Release --no-build --no-restore }
)

foreach ($cmd in $commands) {
    $cmdName = $cmd.ToString().Trim('{}').Trim()
    Write-Host "Executing: $cmdName" -ForegroundColor Yellow
    try {
        & $cmd
        Write-Host "SUCCESS: $cmdName" -ForegroundColor Green
    } catch {
        Write-Host "FAILED: $cmdName" -ForegroundColor Red
        Write-Host "Error: $_" -ForegroundColor Red
        Stop-Transcript
        exit 1
    }
}

Write-Host "=== PowerShell Build Script Completed Successfully ===" -ForegroundColor Cyan
Stop-Transcript

