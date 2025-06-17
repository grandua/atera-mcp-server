<#
.SYNOPSIS
Securely stores Atera API key using Windows DPAPI encryption
.DESCRIPTION
Stores API key in %USERPROFILE%\.atera\apikey.xml
Encrypted for current user/machine only
#>
param([Parameter(Mandatory=$true)][SecureString]$ApiKey)

try {
    # Create secure storage directory
    $secureDir = Join-Path $env:USERPROFILE '.atera'
    if (-not (Test-Path $secureDir)) {
        New-Item -ItemType Directory -Path $secureDir -Force | Out-Null
    }

    # Store encrypted credential
    $credential = New-Object System.Management.Automation.PSCredential('AteraApiKey', $ApiKey)
    $credential | Export-Clixml -Path (Join-Path $secureDir 'apikey.xml') -Force

    # Set for current session
    $env:Atera__ApiKey = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($ApiKey)
    )

    Write-Host "Success: API key securely stored in $secureDir" -ForegroundColor Green
    return $true
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
    return $false
}
