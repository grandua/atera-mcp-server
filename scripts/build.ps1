# Build script for Windows

#Requires -Version 7.0

# Navigate to project root
Set-Location -Path "$PSScriptRoot/.."

# Restore dependencies
dotnet restore AteraMcp.sln

# Build solution
dotnet build AteraMcp.sln -c Release --no-restore

# Run tests
dotnet test AteraMcp.sln -c Release --no-build --no-restore
