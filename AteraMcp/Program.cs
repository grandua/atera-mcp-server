using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AteraApi.DataAccess;

namespace AteraMcp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Expose AteraGateway (and thus IConfiguration) to DI.
        // No extra set-up is required; the gateway itself builds its HttpClient.
        builder.Services.AddSingleton<AteraApiGateway>();

        builder.Services
            .AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();      // picks up AgentListTool automatically

        var host = builder.Build();
        await host.RunAsync();
    }
}