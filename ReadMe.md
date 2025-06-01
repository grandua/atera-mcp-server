### Dcocumentation

**Semantic Kernel (SK):**  
[https://github.com/microsoft/semantic-kernel](https://github.com/microsoft/semantic-kernel)

**MS MCP SDK:**  
[https://github.com/modelcontextprotocol/csharp-sdk](https://github.com/modelcontextprotocol/csharp-sdk)

### Working MCP Server Config Examples

Note: `--no-build` flag is required, otherwise connection errors may occur.

```json
"PlannerMcp": {
  "command": "dotnet",
  "args": [
    "run",
    "--project",
    "C:\\Work\\Projects\\Planner-Mcp\\PlannerMcp\\PlannerMcp.csproj",
    "--no-build"
  ],
  "disabled": false,
  "autoApprove": [],
  "debug": true,
  "options": {
    "shell": true,
    "cwd": "C:\\Work\\Projects\\Planner-Mcp"
  },
  "env": {
    "DOTNET_ENVIRONMENT": "Development"
  }
},
"PlannerMcp-minimal-working-config": {
  "command": "dotnet",
  "args": [
    "run",
    "--project",
    "C:\\Work\\Projects\\Planner-Mcp\\PlannerMcp\\PlannerMcp.csproj",
    "--no-build"
  ]
}