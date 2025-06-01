using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Server; // Added for MCP SDK attributes
using System.ComponentModel; // Added for Description attribute
using System.Threading.Tasks;
using PlannerMcp;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var host = builder.Build();

// Modern SK kernel initialization
var kernel = Kernel.CreateBuilder()
    // .AddOpenAIChatCompletion(modelId: "your-model", apiKey: "your-key")
    .Build();

await PromptOptimizer.RunExample(kernel);

await host.RunAsync();

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from C#: {message}";

    [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
    public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());
}