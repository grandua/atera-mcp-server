using System.Net;
using System.Text.Json;
using Atera.Model;
using Xunit;
using Moq;
using Moq.Protected;

namespace AteraApi.DataAccess.UnitTests;

public class AteraApiGatewayFacts
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly AteraGateway _gateway;

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
                    && req.RequestUri.ToString().Contains("/api/v3/agents")), // Basic check
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _gateway.GetAgentListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("TestPC1", result.First().MachineName);
        // Add more assertions as needed
    }
}