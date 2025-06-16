# Build script for Windows

# Restore dependencies
dotnet restore

# Build solution
dotnet build -c Release --no-restore

# Run tests
dotnet test --no-build --verbosity normal
