#!/bin/bash
set -ex

# Define log file and start logging
LOG_FILE="./build-script.log"
exec > >(tee -a "$LOG_FILE") 2>&1

# ANSI color codes
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
RED='\033[0;31m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

START_TIME=$(date +%s)

echo -e "${CYAN}=== Bash Build Script Started ===${NC}"
echo -e "${YELLOW}Start Time: $(date)${NC}\n"

# --- Configure API Key ---
# 1. Check for Atera__ApiKey environment variable
if [ -n "$Atera__ApiKey" ]; then
    MASKED_API_KEY="****${Atera__ApiKey: -4}"
    echo -e "${GREEN}Using Atera__ApiKey from environment (ends with: ${MASKED_API_KEY})${NC}"
# 2. Fallback to .atera_apikey file in project root
elif [ -f ".atera_apikey" ]; then
    RAW_API_KEY=$(cat ".atera_apikey" | tr -d '\r\n') # Ensure no newlines
    if [ -n "$RAW_API_KEY" ]; then
        export Atera__ApiKey="$RAW_API_KEY"
        MASKED_API_KEY="****${Atera__ApiKey: -4}"
        echo -e "${GREEN}Loaded Atera__ApiKey from ./.atera_apikey file (ends with: ${MASKED_API_KEY})${NC}"
    else
        echo -e "${RED}ERROR: ./.atera_apikey file is empty. API key not configured.${NC}"
    fi
# 3. If neither is found, warn
else
    echo -e "${YELLOW}Warning: Atera__ApiKey environment variable is not set and ./.atera_apikey file not found. Integration tests may fail.${NC}"
fi

# Navigate to the ACTUAL mounted directory in act container
CONTAINER_WORKSPACE="/mnt/c/Work/Projects/Fiverr/AteraMcpServer"
echo -e "${YELLOW}Navigating to: $CONTAINER_WORKSPACE${NC}"
cd "$CONTAINER_WORKSPACE" || {
  echo -e "${RED}ERROR: Failed to navigate to $CONTAINER_WORKSPACE${NC}"
  exit 1
}
echo -e "${GREEN}Current directory: $(pwd)${NC}\n"

# Debug
echo -e "${YELLOW}--- Debug Info ---${NC}"
pwd
ls -la
echo ""

# Verify solution exists
echo -e "${YELLOW}Verifying solution file: AteraMcp.sln${NC}"
if [ ! -f "AteraMcp.sln" ]; then
    echo -e "${RED}ERROR: Solution file not found at: $(pwd)/AteraMcp.sln${NC}"
    exit 1
fi
echo -e "${GREEN}Solution file found.${NC}\n"

# Build commands
echo -e "${YELLOW}Executing: dotnet restore AteraMcp.sln${NC}"
dotnet restore "AteraMcp.sln"
if [ $? -ne 0 ]; then
    echo -e "${RED}FAILED: dotnet restore${NC}"
    exit 1
fi
echo -e "${GREEN}SUCCESS: dotnet restore${NC}\n"

echo -e "${YELLOW}Executing: dotnet build AteraMcp.sln -c Release --no-restore${NC}"
dotnet build "AteraMcp.sln" -c Release --no-restore
if [ $? -ne 0 ]; then
    echo -e "${RED}FAILED: dotnet build${NC}"
    exit 1
fi
echo -e "${GREEN}SUCCESS: dotnet build${NC}\n"

echo -e "${YELLOW}Executing: dotnet test AteraMcp.sln -c Release --no-build --no-restore${NC}"
dotnet test "AteraMcp.sln" -c Release --no-build --no-restore
if [ $? -ne 0 ]; then
    echo -e "${RED}FAILED: dotnet test${NC}"
    exit 1
fi
echo -e "${GREEN}SUCCESS: dotnet test${NC}\n"

END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))

echo -e "${CYAN}=== Bash Build Script Completed Successfully ===${NC}"
echo -e "${GREEN}Total duration: $((DURATION / 60)) minutes $((DURATION % 60)) seconds${NC}"
