## About Project

This project is to create .NET9/MCP SDK/SK project MCP server 
for Atera API ticket management.

Client provided a Python project for the same purpose
and it can found and used for reference here: 
C:\Work\Projects\Fiverr\Python-reference-atera-mcp-server\.
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
1. Create C# solution with Clean Architecture (Domain, Application, Data Access layers)
2. Integrate Semantic Kernel and Atera API client with secure credential storage
3. Implement `GetAgentList` MCP tool with unit tests
4. Configure CI/CD pipeline (GitHub Actions) for local Docker deployment

**Acceptance Criteria:**  
✅ C# solution builds successfully with 100% test pass rate for initial tool  
✅ `GetAgentList` returns valid data via MCP client (Claude Desktop test)  
✅ CI/CD pipeline automatically deploys to your local environment on `main` branch push


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
  "command": "dotnet",
  "args": [
    "run",
    "--project",
    "C:\\Work\\Projects\\Atera-Mcp\\AteraMcp\\AteraMcp.csproj",
    "--no-build"
  ],
  "disabled": false,
  "autoApprove": [],
  "debug": true,
  "options": {
    "shell": true,
    "cwd": "C:\\Work\\Projects\\Atera-Mcp"
  },
  "env": {
    "DOTNET_ENVIRONMENT": "Development"
  }
},
"AteraMcp-minimal-working-config": {
  "command": "dotnet",
  "args": [
    "run",
    "--project",
    "C:\\Work\\Projects\\Atera-Mcp\\AteraMcp\\AteraMcp.csproj",
    "--no-build"
  ]
}