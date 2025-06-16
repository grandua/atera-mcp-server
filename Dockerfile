# ─────────────── BUILD STAGE ───────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1) Copy solution and ONLY *.csproj files to leverage Docker layer caching.
COPY *.sln ./
COPY AteraMcp/*.csproj AteraMcp/
COPY Atera.Model/*.csproj Atera.Model/
COPY AteraApi.DataAccess/*.csproj AteraApi.DataAccess/

# 2) Restore dependencies for the main application project only.
RUN dotnet restore AteraMcp/AteraMcp.csproj

# 3) Copy the remainder of the source tree and publish.
COPY . .
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish AteraMcp/AteraMcp.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ─────────────── RUNTIME STAGE ───────────────
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

# Copy the published output from build stage.
COPY --from=build /app/publish .

# Default command — stdio MCP server
ENTRYPOINT ["dotnet", "AteraMcp.dll"]
