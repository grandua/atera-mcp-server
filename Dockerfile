# ─────────────── BUILD STAGE ───────────────
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore && \
    dotnet build -c Release --no-restore

# Publish stage 
FROM build AS publish
RUN dotnet publish -c Release -o /app

# Runtime stage - corrected for console app
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AteraMcp.dll"]
