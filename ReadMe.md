## About Project

This project is to create .NET9/MCP SDK/SK project MCP server 
for Atera API ticket management.

See also C:\Work\Projects\Fiverr\AteraMcpServer\Clients-project-overview-and-acceptance-criteria.md
and C:\Work\Projects\Fiverr\AteraMcpServer\Python-Reference-Code-Analysis.md


### Python Project from Client for Reference
Client provided a Python project for the same purpose
and it can found and used for reference here:
C:\Work\Projects\Fiverr\Python-reference-atera-mcp-server\


## Milestone 1: Python Reference Code Analysis

### GetAgentList API Analysis

Based on analysis of the Python reference code at `C:\Work\Projects\Fiverr\Python-reference-atera-mcp-server\`:

#### Endpoint Details
- Base URL: `https://app.atera.com`
- Endpoint: `/api/v3/agents` 
- Method: GET
- Authentication: API Key in `X-API-KEY` header

#### Request Parameters
- Optional query params:
  - `page`: Page number for pagination (default: 1)
  - `itemsInPage`: Items per page (default: 100)

#### Response Structure
```json
{
  "items": [
    {
      "agentID": "integer",
      "customerID": "integer", 
      "customerName": "string",
      "machineID": "string",
      "machineName": "string",
      "deviceType": "string",
      "domain": "string",
      "onlineStatus": "boolean",
      "lastRebootTime": "datetime",
      "lastSeenDateTime": "datetime",
      "operatingSystem": "string",
      "ipAddress": "string",
      "externalIP": "string",
      "snmpEnabled": "boolean",
      "monitoringThreshold": "integer"
    }
  ],
  "itemsCount": "integer",
  "totalItems": "integer"
}
```

#### Key Fields for Domain Model
Essential fields to map in our C# domain model:
- `agentID`: Unique identifier for the agent
- `customerID`: Associated customer ID
- `customerName`: Customer business name
- `machineName`: Name of the monitored machine
- `onlineStatus`: Current connection status
- `lastRebootTime`: Last system restart
- `operatingSystem`: OS details
- `ipAddress`: Internal IP
- `externalIP`: Public IP

This analysis will inform our C# domain model design and API integration layer implementation.C:\Work\Projects\Fiverr\Python-reference-atera-mcp-server\.
But we should not copy any code or structures from that Python project blindly, 
as we will follow Clean Architecture, Rich Domain Model, SOLID, and TDD/ATDD.  

### To Do

#### Full Project Scope
This offer covers the Pro Tier deliverables for your Atera MCP Server, with a clear division of responsibilities:

**My Deliverables (Pro Tier):**
Delivery of a complete C# MCP Server, including:
• 19 fully tested C# MCP Tools (11 GET, 8 mutation) for the Atera API.
• 8 MindsDB Skills configured for your database queries.
• A comprehensive test suite (benchmarked against the complexity of the original 268 paths) to ensure high reliability.
• A CI/CD pipeline for deploying to two local environments.


#### Current Milestone: **Milestone 1: Core MCP Foundation & Initial API Tool (V0)**
**Steps:**
1. Create C# solution with Clean Architecture (Domain, API, Data Access layers)
2. Integrate Semantic Kernel and Atera API client with secure credential storage
3. Implement `GetAgentList` MCP tool with unit tests
4. Configure CI/CD pipeline (GitHub Actions) for local Docker deployment

**Acceptance Criteria:**  
✅ C# solution builds successfully with 100% test pass rate for initial tool  
✅ `GetAgentList` returns valid data via MCP client (Claude Desktop test)  
✅ CI/CD pipeline automatically deploys to your local environment on `main` branch push

#### Milestone 1 Current Status

Implemented:

✅ C# solution structure appears created with:
Domain layer (AteraMcp)
Data Access layer (AteraApi.DataAccess)
Test projects (*.UnitTests, *.IntegrationTests)
✅ GetAgentList tool implementation started:
Core tool class exists (AgentListTool.cs)
Unit tests exist (AgentListToolFacts.cs)
Integration tests exist (AteraMcpServerFacts.cs)
✅ API client integration:
AteraApiGateway class exists with tests
Authentication likely implemented (based on test files)
Not Yet Implemented:

❌ CI/CD pipeline configuration (no GitHub Actions/Docker files visible)
❌ Complete test coverage (need to verify 100% pass rate)
❌ Final deployment verification (needs Claude Desktop test)
The core foundation is in place with the solution structure and initial tool implementation. The remaining work focuses on pipeline setup and validation.


### Testing Notes
- Start server with `dotnet run --no-build` in test fixture.
- Use **JsonRpc.Client** to send requests and assert responses.
- Tests run sequentially to avoid port conflicts.

---

### 3rd Party Dependencies

**Semantic Kernel (SK):**  
[https://github.com/microsoft/semantic-kernel](https://github.com/microsoft/semantic-kernel)

**MS MCP SDK:**  
[https://github.com/modelcontextprotocol/csharp-sdk](https://github.com/modelcontextprotocol/csharp-sdk)

### Working MCP Server Config Examples

Note: `--no-build` flag is required, otherwise connection errors may occur.

```json
    "AteraMcp": {
      "command": "C:\\Users\\Grand\\.dotnet\\dotnet.exe",
      "args": [
        "run",
        "--project",
        "C:\\Work\\Projects\\Fiverr\\AteraMcpServer\\AteraMcp\\AteraMcp.csproj",
        "--no-build"
      ],
      "timeout": 60,
      "transportType": "stdio",
      "env": {
        "DOTNET_ENVIRONMENT": "Development",
        "Atera__ApiKey": "6a..."
      },
      "disabled": false
    }
```

## Environment Setup

1. Create a `.env` file for local testing:
```ini
DOCKER_USERNAME=your_dockerhub_username
DOCKER_PASSWORD=your_dockerhub_password
```

2. For GitHub Actions, add repository secrets:
- `DOCKER_USERNAME`: Your Docker Hub username
- `DOCKER_PASSWORD`: Your Docker Hub password/access token

## Setting Up GitHub Secrets

To build and push Docker images from GitHub Actions, you need to set up secrets for your Docker Hub credentials:

1. Navigate to your GitHub repository.
2. Click on **Settings**.
3. In the left sidebar, click on **Secrets and variables** > **Actions**.
4. Click **New repository secret**.
5. Create the following secrets:
   - `DOCKER_USERNAME`: Your Docker Hub username
   - `DOCKER_PASSWORD`: Your Docker Hub password or access token

These secrets will be used by the GitHub Actions workflow to authenticate with Docker Hub.

### Analyzing Test Logs

The CI/CD test scripts (`test-ci-cd.sh` and `test-ci-cd.ps1`) generate detailed logs in the `Logs/` directory. Here’s how to effectively read them using PowerShell.

**Basic Command:**

The standard command to view a file's content is `Get-Content`:

```powershell
Get-Content .\Logs\test-ci-cd-YYYYMMDD-HHMMSS.log
```

**Gotchas and Best Practices:**

1.  **Slow on Large Files:** The test scripts can generate very large log files. Running `Get-Content` on a large file can be slow and make it seem like the command is stuck. To quickly check the most recent activity, use the `-Tail` parameter to view the last N lines.

    ```powershell
    # Shows the last 50 lines of the log, which is much faster
    Get-Content .\Logs\<log-file-name>.log -Tail 50
    ```

2.  **Real-time Feedback:** The PowerShell script now uses `Tee-Object` to show command output in the console in real-time *while also* writing it to the log file. This is crucial for monitoring the progress of long-running commands (like `dotnet test` or `docker build`) and confirming they aren’t stuck.

3.  **`.gitignore` and Tooling:** The project's `.gitignore` file contains the rule `*.log`. This is a best practice, but it can prevent some IDE tools or automated agents from accessing log files. If a tool reports that it cannot access a log file, the workaround is to use a direct `Get-Content` command from a standard PowerShell terminal, as this is not subject to the same restrictions.

## Docker Containerization

The AteraMcp server has been dockerized for easy deployment and CI/CD integration.

### Key Features
- Multi-stage build for optimal image size
- .NET 9 runtime optimized for console apps
- Proper layer caching for fast rebuilds
- Stdio communication for MCP protocol

### Building the Image
```bash
docker build -t atera-mcp .
```

### Running the Container
```bash
# Basic run (stdio mode)
docker run -it atera-mcp

# With environment variables
docker run -it -e ATERA_API_KEY=your_key atera-mcp
```

### Testing the Container
```bash
# Send JSON-RPC 2.0 command
echo '{"jsonrpc":"2.0","method":"mcp-version","id":1}' | docker run -i atera-mcp
```

### CI/CD Integration
Example GitHub Actions workflow:
```yaml
steps:
  - name: Build Docker image
    run: docker build -t atera-mcp .
    
  - name: Run tests
    run: |
      echo '{"jsonrpc":"2.0","method":"echo","params":{"message":"test"},"id":1}' \
        | docker run -i -e ATERA_API_KEY=${{ secrets.ATERA_API_KEY }} atera-mcp
```

### Implementation Notes
1. Uses JSON-RPC 2.0 over stdio
2. Environment variables for configuration
3. `.dockerignore` optimizes build context

## Testing

### One-shot CI/CD Verification

Use the helper scripts to execute the full cross-environment test plan locally.

#### Linux / WSL

Run from **Windows Terminal** using the built-in `wsl` command (no need to open a separate WSL window):

```powershell
# Windows PowerShell / CMD
wsl bash -c "cd /mnt/c/Work/Projects/Fiverr/AteraMcpServer && chmod +x scripts/test-ci-cd.sh && ./scripts/test-ci-cd.sh"
```

Or, if you are already **inside** a WSL shell:

```bash
cd /mnt/c/Work/Projects/Fiverr/AteraMcpServer
chmod +x scripts/test-ci-cd.sh   # first time only
./scripts/test-ci-cd.sh
```

#### Windows PowerShell
```powershell
# From a Developer PowerShell prompt
# -ApiKey parameter is optional if the variable or .atera_apikey exists
.\scripts\test-ci-cd.ps1 -ApiKey 'your_api_key_here'
```

The scripts perform:
1. Debug & Release builds/tests via `build.sh` / `build.ps1`.
2. CI pipeline emulation with [`act`](https://github.com/nektos/act).
3. Docker image build and in-container integration tests.
4. (PowerShell) Native Windows Debug/Release test runs.

### API Key Handling
The project looks for the Atera API key in this order:
1. **Environment variable** `Atera__ApiKey` (preferred).
2. **`.atera_apikey`** plaintext file in the repository root (convenience only—git-ignored by default).

If neither is provided, integration tests that reach the Atera API will be skipped/fail gracefully.

### Automatic act Caching

The test script will automatically:
1. Check for a locally cached `act` in `.bin/` directory
2. Use system-wide `act` if available
3. Install and cache `act` locally if missing

No manual installation is required - the script handles everything.

## Building and Running

### Prerequisites
- .NET 9 SDK (version 9.0.301 or later)
- Docker Desktop with WSL 2 integration enabled
  - Go to Settings -> Resources -> WSL Integration and enable for your distro
- GitHub CLI (optional, for CI simulation)
- PowerShell 7+ (for Windows scripts)
- Bash (for Linux/WSL scripts)
- MCP .NET SDK

### Compilation Instructions

1. Navigate to the project directory:
   ```powershell
   cd C:\Work\Projects\Fiverr\AteraMcpServer
   ```

2. Build the solution using .NET 9:
   ```powershell
   C:\Users\Grand\.dotnet\dotnet.exe build
   ```

3. Run tests:
   ```powershell
   C:\Users\Grand\.dotnet\dotnet.exe test
   ```

## .NET 9 Compatibility

- The project targets `net9.0` and uses modern .NET features
- The MCP SDK works perfectly with .NET 9 - all warnings about .NET 9 compatibility can be safely ignored
- Verified working with .NET 9.0.301 SDK

### Troubleshooting

If you encounter build issues:
1. Clear NuGet caches:
   ```powershell
   dotnet nuget locals all --clear
   ```

2. Restore packages:
   ```powershell
   dotnet restore
   ```

3. Rebuild completely:
   ```powershell
   dotnet clean
   dotnet build
   ```

## Configuration Setup

1. **API Key Configuration**:
   - For development, use user secrets:
     ```bash
     dotnet user-secrets init
     dotnet user-secrets set "Atera:ApiKey" "your_api_key_here"
     ```
   - For production, set environment variable:
     ```
     Atera__ApiKey=your_api_key_here
     ```

2. **Configuration Files**:
   - `appsettings.json`: Template configuration (checked into source control)
   - `appsettings.Development.json`: Local overrides (gitignored)

## Configuration

API keys should be stored in user secrets (shared with AteraApi.DataAccess project):
```json
{
  "Atera": {
    "ApiKey": "your_api_key_here"
  }
}