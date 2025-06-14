using System.Net;
using System.Text.Json;
using Atera.Model;
using Xunit;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Configuration;

namespace AteraApi.DataAccess.UnitTests;

public class AteraApiGatewayFacts
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly AteraGateway _gateway;
    private readonly Mock<IConfiguration> _mockConfig;

    public AteraApiGatewayFacts()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://app.atera.com")
        };
        
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(x => x["Atera:ApiKey"]).Returns("test-api-key");
        
        _gateway = new AteraGateway(_mockConfig.Object, _httpClient);
    }

    [Fact]
    public async Task GetAgentListAsyncSuccess()
    {
        // Arrange
        var expectedAgents = new List<Agent>
        {
            new Agent { AgentID = 1, MachineName = "TestPC1", Online = true },
            new Agent { AgentID = 2, MachineName = "TestServer1", Online = false }
        };
        var apiResponse = new AgentList { Items = expectedAgents, TotalItems = 2, Page = 1, ItemsInPage = 2, TotalPages = 1 };
        var jsonResponse = JsonSerializer.Serialize(apiResponse);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get 
                    && req.RequestUri.ToString().Contains("/api/v3/agents")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = (await _gateway.GetAgentListAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("TestPC1", result[0].MachineName);
    }
}