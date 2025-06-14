using Microsoft.Extensions.Configuration;
using System.Net;
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
        private const int DefaultPageSize = 100;
        private const int MaxPageSize = 1000;

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

        public async Task<IEnumerable<Agent>> GetAgentListAsync(
            int page = 1, 
            int itemsInPage = DefaultPageSize,
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than 0");
            itemsInPage = Math.Clamp(itemsInPage, 1, MaxPageSize);

            try
            {
                var requestUri = $"/api/v3/agents?page={page}&itemsInPage={itemsInPage}";
                var response = await _httpClient.GetAsync(requestUri, cancellationToken);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Enumerable.Empty<Agent>();
                }

                response.EnsureSuccessStatusCode();
                
                var agentList = await response.Content.ReadFromJsonAsync<AgentList>(
                    cancellationToken: cancellationToken);
                    
                return agentList?.Items ?? Enumerable.Empty<Agent>();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // TODO: Add retry logic or proper rate limit handling
                throw new AteraApiException("API rate limit exceeded", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new AteraApiException("Error calling Atera API", ex);
            }
        }
    }

    public class AteraApiException : Exception
    {
        public AteraApiException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}