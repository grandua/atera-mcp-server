using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Atera.Model;

namespace AteraApi.DataAccess
{
    public class AteraGateway
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string ApiBaseUrl = "https://app.atera.com";

        public AteraGateway(IConfiguration configuration, HttpClient? httpClient = null)
        {
            _configuration = configuration;
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", 
                _configuration["Atera:ApiKey"]);
        }

        public async Task<IEnumerable<Agent>> GetAgentListAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("/api/v3/agents", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var agentList = await response.Content.ReadFromJsonAsync<AgentList>(
                cancellationToken: cancellationToken);
                
            return agentList?.Items ?? Enumerable.Empty<Agent>();
        }
    }
}