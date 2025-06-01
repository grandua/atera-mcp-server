Working MCP server config sections examples:

Note: "--no-build" flag is required, otherwise I see connection errors.


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