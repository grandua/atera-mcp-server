using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AteraApi.DataAccess;

namespace AteraMcp.IntegrationTests;

public class AgentListToolFacts : IDisposable
{
    readonly IHost host;
    
    public AgentListToolFacts()
    {
        var hostBuilder = Host.CreateApplicationBuilder(new string[0]);
        
        // Load configuration from user secrets
        hostBuilder.Configuration.AddUserSecrets<AgentListToolFacts>();
        
        // Also try environment variables as fallback
        hostBuilder.Configuration.AddEnvironmentVariables();
        
        if (string.IsNullOrEmpty(hostBuilder.Configuration["Atera:ApiKey"]))
        {
            throw new InvalidOperationException(
                "Atera API key not configured. Set via user secrets or Atera__ApiKey environment variable");
        }
        
        hostBuilder.Services.AddSingleton<AteraApiGateway>();
        hostBuilder.Services.AddSingleton<AteraTools>();
        
        host = hostBuilder.Build();
    }
    
    public void Dispose() => host?.Dispose();
    
    [Fact]
    public async Task GetAgentList_ReturnsValidApiResponse()
    {
        var agentListTool = host.Services.GetRequiredService<AteraTools>();
        var result = await agentListTool.GetAgentList();
        
        Assert.NotNull(result);
        Assert.StartsWith("[", result); // Should be a JSON array
        Assert.Contains("\"AgentID\":", result);
    }
    
    [Fact]
    public async Task GetAgentList_ReturnsAgentData()
    {
        var agentListTool = host.Services.GetRequiredService<AteraTools>();
        var result = await agentListTool.GetAgentList();
        
        Assert.Contains("\"AgentID\":", result);
        Assert.Contains("\"MachineName\":", result);
    }
}
