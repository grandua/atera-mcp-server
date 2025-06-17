#!/usr/bin/env bash
set -e

# Create Logs directory if it doesn't exist
LOG_DIR="./Logs"
mkdir -p "$LOG_DIR"
LOG_FILE="$LOG_DIR/test-ci-cd-$(date +'%Y%m%d-%H%M%S').log"
exec > >(tee -a "$LOG_FILE") 2>&1

echo "=== Script started at $(date) ==="
echo "Logging to: $LOG_FILE"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
CYAN='\033[0;36m'
NC='\033[0m'

# Header
log_header() {
  echo -e "${CYAN}\n=== $1 ===${NC}"
}

# Docker command with WSL fallback
docker_cmd() {
    # First try native Docker
    if command -v docker &> /dev/null; then
        docker "$@"
    # Fallback to Windows Docker
    elif [ -f "/mnt/c/Program Files/Docker/Docker/resources/bin/docker.exe" ]; then
        "/mnt/c/Program Files/Docker/Docker/resources/bin/docker.exe" "$@"
    else
        echo -e "${RED}ERROR: Docker not found. Please enable WSL integration in Docker Desktop${NC}"
        echo -e "${YELLOW}Instructions: https://docs.docker.com/desktop/wsl/ ${NC}"
        exit 1
    fi
}

# Load API key fallback
if [ -z "$Atera__ApiKey" ]; then
    KEY_FILE="$(dirname "${BASH_SOURCE[0]}")/../.atera_apikey"
    if [ -f "$KEY_FILE" ]; then
        export Atera__ApiKey=$(<"$KEY_FILE")
        echo -e "${GREEN}✓ Loaded Atera__ApiKey from .atera_apikey${NC}"
    else
        echo -e "${YELLOW}⚠ Warning: Atera__ApiKey not set and .atera_apikey file not found${NC}"
    fi
fi

# Ensure we're in the correct directory (WSL compatible)
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT" || {
    echo -e "${RED}ERROR: Failed to navigate to $REPO_ROOT${NC}"
    exit 1
}

# Skipping CI simulation for now due to issues with Windows jobs
# log_header "Skipping CI/CD Pipeline Simulation (act) due to issues with Windows jobs"

# Uncomment below to run CI simulation
install_act() {
    echo "Installing act to .bin directory..."
    mkdir -p .bin
    curl -sL https://raw.githubusercontent.com/nektos/act/master/install.sh | bash -s -- -b .bin
    echo "✓ act installed"
}

# Main execution (new order)
log_header "1. CI/CD Pipeline Simulation (Linux only)"

# The workflow filtering has been removed to allow testing the full matrix.
# A filtered workflow is no longer created or used.

# Create filtered workflow excluding Windows jobs because 'act' does not support them.
FILTERED_WORKFLOW="$REPO_ROOT/.github/workflows/build-filtered.yml"
sed 's/, windows-latest//' "$REPO_ROOT/.github/workflows/build.yml" > "$FILTERED_WORKFLOW"

# Run act with filtered workflow
if [ -f "$REPO_ROOT/.bin/act" ]; then
    "$REPO_ROOT/.bin/act" -j build-and-test \
      -W "$FILTERED_WORKFLOW" \
      -s ATERA_API_KEY="$Atera__ApiKey" \
      -P ubuntu-latest=catthehacker/ubuntu:act-latest \
      --rebuild \
      -v \
      --env ACTIONS_RUNNER_DEBUG=false \
      --env GITHUB_WORKSPACE=/mnt/c/Work/Projects/Fiverr/AteraMcpServer
elif command -v act &> /dev/null; then
    act -j build-and-test \
      -W "$FILTERED_WORKFLOW" \
      -s ATERA_API_KEY="$Atera__ApiKey" \
      -P ubuntu-latest=catthehacker/ubuntu:act-latest \
      --rebuild \
      -v \
      --env ACTIONS_RUNNER_DEBUG=false \
      --env GITHUB_WORKSPACE=/mnt/c/Work/Projects/Fiverr/AteraMcpServer
else
    install_act
    "$REPO_ROOT/.bin/act" -j build-and-test \
      -W "$FILTERED_WORKFLOW" \
      -s ATERA_API_KEY="$Atera__ApiKey" \
      -P ubuntu-latest=catthehacker/ubuntu:act-latest \
      --rebuild \
      -v \
      --env ACTIONS_RUNNER_DEBUG=false \
      --env GITHUB_WORKSPACE=/mnt/c/Work/Projects/Fiverr/AteraMcpServer
fi

# Clean up
rm "$FILTERED_WORKFLOW"

# Add temporary test credentials for local Docker operations (will be ignored in real CI)
export DOCKER_USERNAME="testuser"
export DOCKER_PASSWORD="testpass"
# Add warning message
echo "[NOTE] Using test Docker credentials - these will NOT work for actual pushes" >&2

log_header "2. Docker Build & Test"
docker_cmd build -t atera-mcp-server .
docker_cmd run --rm -e Atera__ApiKey="$Atera__ApiKey" atera-mcp-server dotnet test AteraMcp.IntegrationTests.dll

log_header "3. WSL/Debug Build & Test"
BUILD_CONFIGURATION=Debug ./scripts/build.sh

log_header "4. WSL/Release Build & Test"
BUILD_CONFIGURATION=Release ./scripts/build.sh

echo -e "${GREEN}\n✓ All tests completed successfully${NC}"

echo "=== Script completed at $(date) ==="
