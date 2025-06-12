## About Project

This project is to convert .NET8/MCP SDK project copied from Planner MCP server 
into .NET project for Atera API ticket management MCP server.
PlannerMcp has a fully working Echo 101 MCP tool.
Eventually we will convert this PlannnerMCP MCP server into Atera MCP server for Atera API.


## To Do

At this stage the **sole goal** is to prove that this codebase functions as a compliant **MCP server**.  
We will add **1‒3 high-value integration tests** that cover the critical MCP flows and nothing else.

### 🎯 Target Integration Tests
| # | Scenario | MCP Method(s) | Expected Outcome |
|---|-----------|---------------|------------------|
| 1 | **Server handshake** | `implementation/get` | Server responds with non-empty `name` & `version` |
| 2 | **Tool discovery** | `tool/list` | Response array contains `echo` tool with correct signature |
| 3 | **Echo round-trip** | `tools/call` (params: `{"name": "Echo", ...}`)| Input text is returned, wrapped in a content object |

All tests will be written with **xUnit** and executed via `dotnet test --filter "Category=Integration"`.

---

### 1️⃣  Short-Term Goals (MVP)
- [ ] Green-field integration test proving the MCP server can **start ➜ initialise ➜ respond**  
  - Command: `dotnet test --filter "Category=Integration"`
- [ ] Basic logging via **Microsoft.Extensions.Logging**

---

### CRC Cards (minimal)
| Class | Responsibilities | Collaborators |
|-------|------------------|---------------|
| `Program` | Configure DI, register tools, start MCP server via stdio transport | `EchoTool`, `HelloWorldMcpServer` |
| `EchoTool` | Implements `Echo` and `ReverseEcho` MCP tools | `Program` |
| `HelloWorldMcpServer` | Implements `SayHello` MCP tool | `Program` |
| `PromptOptimizer` | Demonstrates SK usage (non-critical) | `Program` |

---

### Navigation Map
```mermaid
flowchart TD
    Client -->|"implementation/get" & "tool/list"| Program
    Client -->|"tools/call" (params: {name:'Echo', ...})| EchoTool
    Client -->|"tool/sayHello.call"| HelloWorldMcpServer
```

#### Call Stack & Parameters (happy-path)
1. **Handshake**  
   • Request: `implementation/get {}`  
   • Response: `{ name: string, version: string }`
2. **Tool Catalogue**  
   • Request: `tool/list {}`  
   • Response: `{ "tools": [ { "name": "Echo", ... }, { "name": "ReverseEcho", ... }, { "name": "SayHello", ... } ] }`
3. **Echo Round-Trip**  
   • Request: `tools/call { "name": "Echo", "arguments": { "message": "hello" } }`  
   • Program routes to `EchoTool.Echo(message:"hello")`  
   • Response: `{ "result": { "content": [{ "type": "text", "text": "Hello from C#: hello" }], "isError": false } }`
4. **Hello Round-Trip**  
   • Request: `tool/sayHello.call {}`  
   • Program routes to `HelloWorldMcpServer.SayHello()`  
   • Response: `{ result: "Hello World from MCP Server!" }`

---

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