# Docker build script for Windows

#Requires -Version 7.0

# Authenticate with Docker Hub
$env:DOCKER_PASSWORD | docker login -u $env:DOCKER_USERNAME --password-stdin

# Build the image
docker build -t atera-mcp .

# Test the image
Write-Output "Testing built image..."
docker run --rm atera-mcp dotnet --version

Write-Host "Docker image built successfully"
