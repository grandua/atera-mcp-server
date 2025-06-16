#!/bin/bash
# Build script for Linux/macOS

# Restore dependencies
dotnet restore

# Build solution
dotnet build -c Release --no-restore

# Run tests
dotnet test --no-build --verbosity normal
