# Docker build script for Windows

# Build the image
docker build -t atera-mcp .

# Test the image
Write-Output "Testing built image..."
docker run --rm atera-mcp dotnet --version
