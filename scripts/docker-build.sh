#!/bin/bash
set -e

# Authenticate with Docker Hub (requires DOCKER_USERNAME and DOCKER_PASSWORD environment variables)
echo "$DOCKER_PASSWORD" | docker login -u "$DOCKER_USERNAME" --password-stdin

# Build the image
docker build -t atera-mcp .

# Test the image
echo "Testing built image..."
docker run --rm atera-mcp dotnet --version

echo "Docker image built successfully"
