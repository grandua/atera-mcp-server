using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
public class HelloWorldMcpServer
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public string SayHello()
    {
        return "Hello World from MCP Server!";
    }
}
