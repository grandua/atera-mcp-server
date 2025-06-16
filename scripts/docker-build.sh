#!/bin/bash
# Docker build script for Linux/macOS

# Build the image
docker build -t atera-mcp .

# Test the image
echo "Testing built image..."
docker run --rm atera-mcp dotnet --version
