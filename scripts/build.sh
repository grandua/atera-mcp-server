#!/bin/bash
set -ex

# Workspace path handling - works in both GitHub Actions and local act
CI_WORKSPACE="${GITHUB_WORKSPACE:-/mnt/c/Work/Projects/Fiverr/AteraMcpServer}"

# Navigate to workspace
cd "$CI_WORKSPACE" || {
  echo "ERROR: Failed to navigate to workspace: $CI_WORKSPACE"
  exit 1
}

# Verify solution file exists
SOLUTION_FILE="AteraMcp.sln"
if [ ! -f "$SOLUTION_FILE" ]; then
    echo "ERROR: Solution file not found at: $CI_WORKSPACE/$SOLUTION_FILE"
    echo "Directory contents:"
    ls -la
    exit 1
fi

# Build commands
dotnet restore "$SOLUTION_FILE"
dotnet build "$SOLUTION_FILE" -c Release --no-restore
dotnet test "$SOLUTION_FILE" -c Release --no-build --no-restore
