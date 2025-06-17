<#
.SYNOPSIS
    Comprehensive CI/CD test script for Windows PowerShell
#>
param(
    [string]$ApiKey = ''
)

# Colors
$RED = "\033[0;31m"
$GREEN = "\033[0;32m"
$YELLOW = "\033[0;33m"
$CYAN = "\033[0;36m"
$NC = "\033[0m"

function Write-Header($text) {
    Write-Host "`n${CYAN}=== $text ===${NC}"
}

# Load API key: use env var, fallback to .atera_apikey
if (-not $ApiKey) {
    if ($env:Atera__ApiKey) { $ApiKey = $env:Atera__ApiKey }
    else {
        $keyFile = Join-Path $PSScriptRoot '..\.atera_apikey'
        if (Test-Path $keyFile) {
            $ApiKey = (Get-Content $keyFile -Raw).Trim()
            Write-Host "${GREEN}✓ Loaded API key from .atera_apikey file${NC}"
        }
    }
}
if (-not $ApiKey) {
    Write-Host "${YELLOW}⚠ Warning: API key not set; integration tests may fail${NC}"
} else {
    $env:Atera__ApiKey = $ApiKey
}

Write-Header "1. Windows Debug Build & Test"
& dotnet test "AteraMcp.sln" -c Debug --no-restore --verbosity normal

Write-Header "2. Windows Release Build & Test"
& dotnet test "AteraMcp.sln" -c Release --no-restore --verbosity normal

Write-Header "3. Docker Build & Test"
& docker build -t atera-mcp-server .
& docker run --rm -e Atera__ApiKey=$ApiKey atera-mcp-server dotnet test AteraMcp.IntegrationTests.dll --no-build --verbosity normal

Write-Host "${GREEN}`n✓ All Windows tests completed successfully${NC}"
