using System.ComponentModel;
using System.Text.Json;
using AteraApi.DataAccess;
using ModelContextProtocol.Server;

namespace AteraMcp;

// Instance-based MCP tool → DI gives us AteraGateway that is built with the
// app’s IConfiguration.  The class must be annotated with McpServerToolType.
[McpServerToolType]
public sealed class AteraTools
{
    private readonly AteraApiGateway _gateway;

    public AteraTools(AteraApiGateway gateway)
    {
        _gateway = gateway;
    }

    [McpServerTool, Description("Get a list of agents from Atera API.")]
    public async Task<string> GetAgentList()          // ← async is fine; MCP awaits it
    {
        var agents = await _gateway.GetAgentListAsync();     // real call, real config
        return JsonSerializer.Serialize(agents);             // keep string return
    }
}