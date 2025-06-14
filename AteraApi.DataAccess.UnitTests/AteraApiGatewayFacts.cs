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
    readonly Mock<HttpMessageHandler> mockHttpMessageHandler;
    readonly HttpClient httpClient;
    readonly AteraApiGateway gateway;
    readonly Mock<IConfiguration> mockConfig;

    public AteraApiGatewayFacts()
    {
        mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://app.atera.com")
        };
        
        mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["Atera:ApiKey"]).Returns("test-api-key");
        
        gateway = new AteraApiGateway(mockConfig.Object, httpClient);
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

        mockHttpMessageHandler.Protected()
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
        var result = (await gateway.GetAgentListAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("TestPC1", result[0].MachineName);
    }
}