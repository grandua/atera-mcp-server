#!/bin/bash
# set-atera-apikey-wsl.sh
# Usage: ./set-atera-apikey-wsl.sh <your-api-key>
# Sets the Atera__ApiKey environment variable permanently for your WSL user.

if [ -z "$1" ]; then
  echo "Usage: $0 <your-api-key>"
  exit 1
fi

API_KEY="$1"
PROFILE_FILE="$HOME/.bashrc"

# Remove any existing export for Atera__ApiKey
sed -i '/export Atera__ApiKey=/d' "$PROFILE_FILE"
# Add new export
printf '\nexport Atera__ApiKey="%s"\n' "$API_KEY" >> "$PROFILE_FILE"

# Apply immediately for current session
export Atera__ApiKey="$API_KEY"

MASKED_API_KEY="****${API_KEY: -4}"
echo "Atera__ApiKey set permanently in $PROFILE_FILE (ends with: ${MASKED_API_KEY})"
