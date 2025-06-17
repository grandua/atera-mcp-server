#!/usr/bin/env bash
set -e

# Create Logs directory if it doesn't exist
LOG_DIR="$(dirname "$(realpath "$0")")/../Logs"
mkdir -p "$LOG_DIR"
LOG_FILE="$LOG_DIR/build-$(date +'%Y%m%d-%H%M%S').log"
exec > >(tee -a "$LOG_FILE") 2>&1

echo "=== Build started at $(date) ==="
echo "Logging to: $LOG_FILE"

# Configuration
CONFIG=${BUILD_CONFIGURATION:-Release}

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
CYAN='\033[0;36m'
NC='\033[0m'

# Workspace detection - handles both WSL and native paths
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT" || {
    echo -e "${RED}ERROR: Failed to navigate to $REPO_ROOT${NC}"
    exit 1
}

echo -e "${CYAN}=== Bash Build Script Started (Config: $CONFIG) ===${NC}"
echo -e "${YELLOW}Workspace: $(pwd)${NC}"

# API key handling: fallback to .atera_apikey file
if [ -z "$Atera__ApiKey" ]; then
    if [ -f ".atera_apikey" ]; then
        export Atera__ApiKey=$(<".atera_apikey")
        echo -e "${GREEN}Loaded Atera__ApiKey from .atera_apikey file${NC}"
    else
        echo -e "${YELLOW}Warning: Atera__ApiKey not set and .atera_apikey file not found. Integration tests may fail.${NC}"
    fi
fi

# Solution verification
if [ ! -f "AteraMcp.sln" ]; then
    echo -e "${RED}ERROR: Solution file not found in $(pwd)${NC}"
    exit 1
fi

echo -e "${GREEN}Solution verified${NC}"

# Build pipeline

echo -e "${YELLOW}Restoring dependencies...${NC}"
dotnet restore "AteraMcp.sln"

echo -e "${YELLOW}Building solution ($CONFIG configuration)...${NC}"
dotnet build "AteraMcp.sln" -c $CONFIG --no-restore

echo -e "${YELLOW}Running tests ($CONFIG configuration)...${NC}"
dotnet test "AteraMcp.sln" -c $CONFIG --no-build --no-restore

echo -e "${CYAN}Build completed successfully${NC}"

echo "=== Build completed at $(date) ==="
