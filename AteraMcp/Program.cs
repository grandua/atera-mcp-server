using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AteraMcp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        var host = builder.Build();
        await host.RunAsync();
    }
}