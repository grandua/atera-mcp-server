using System.ComponentModel;
using AteraApi.DataAccess;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;

namespace AteraMcp;

public static class AgentListTool
{
    [McpServerTool, Description("Get a list of agents from Atera API.")]
    public static string GetAgentList() => new AteraGateway(new ConfigurationBuilder().Build()).GetAgentListAsync().Result.ToString();
}