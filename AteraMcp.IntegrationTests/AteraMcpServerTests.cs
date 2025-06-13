using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace AteraMcp.IntegrationTests;

[Trait("Category", "Integration")]
public class AteraMcpServerTests : IDisposable
{
    private readonly string _serverPath;
    private readonly ITestOutputHelper _output;

    public AteraMcpServerTests(ITestOutputHelper output)
    {
        _output = output;
        
        var testDir = Path.GetDirectoryName(typeof(AteraMcpServerTests).Assembly.Location);
        _serverPath = Path.GetFullPath(Path.Combine(testDir!, @"..\..\..\..", "AteraMcp", "bin", "Debug", "net8.0", 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "AteraMcp.exe" : "AteraMcp"));

        if (!File.Exists(_serverPath))
        {
            throw new InvalidOperationException($"Could not find AteraMcp executable at: {_serverPath}");
        }
        process = new TestProcess(_serverPath, _output);
        process.Start();
    }

    TestProcess process;
    public void Dispose()
    {
        process.Dispose();
    }

    [Fact]
    public void Echo_ReturnsPrefixedMessage()
    {
        // Arrange
        var testMessage = "integration_test_" + Guid.NewGuid();

        // First do tools/list to see registered methods
        var listResponse = process.SendRequest(new
        {
            jsonrpc = "2.0",
            id = "test_list",
            method = "tools/list",
            @params = new { }
        });
        _output.WriteLine($"Available tools: {listResponse}");

        // Act
        var response = process.SendRequest(new
        {
            jsonrpc = "2.0",
            id = "test_echo",
            method = "tools/call",
            @params = new { name = "Echo", arguments = new { message = testMessage } }
        });

        // Assert
        if (response.RootElement.TryGetProperty("error", out var error))
        {
            Assert.Fail($"Server returned error: {error.GetProperty("message").GetString()}");
        }

        var resultObject = response.RootElement.GetProperty("result");
        var contentArray = resultObject.GetProperty("content");
        var firstContentElement = contentArray[0];
        var actualMessage = firstContentElement.GetProperty("text").GetString();

        Assert.NotNull(actualMessage);
        Assert.Contains(testMessage, actualMessage);
        Assert.Contains("Hello from C#", actualMessage);
    }

    [Fact]
    public void ReverseEcho_ReturnsReversedMessage()
    {
        // Arrange
        var testMessage = "integration_test";

        // First do tools/list to see registered methods
        var listResponse = process.SendRequest(new
        {
            jsonrpc = "2.0",
            id = "test_list",
            method = "tools/list",
            @params = new { }
        });
        _output.WriteLine($"Available tools: {listResponse}");

        // Act
        var response = process.SendRequest(new
        {
            jsonrpc = "2.0",
            id = "test_reverse",
            method = "tools/call",
            @params = new { name = "ReverseEcho", arguments = new { message = testMessage } }
        });

        // Assert
        if (response.RootElement.TryGetProperty("error", out var error))
        {
            Assert.Fail($"Server returned error: {error.GetProperty("message").GetString()}");
        }

        var resultObject = response.RootElement.GetProperty("result");
        var contentArray = resultObject.GetProperty("content");
        var firstContentElement = contentArray[0];
        var actualMessage = firstContentElement.GetProperty("text").GetString();

        Assert.NotNull(actualMessage);
        Assert.Equal(new string(testMessage.Reverse().ToArray()), actualMessage);
    }

    [Fact]
    public void Tools_ListsAvailableTools()
    {
        // Act
        var response = process.SendRequest(new
        {
            jsonrpc = "2.0",
            id = "test_tool_list",
            method = "tools/list",
            @params = new { }
        });

        // Assert
        if (response.RootElement.TryGetProperty("error", out var error))
        {
            Assert.Fail($"Server returned error: {error.GetProperty("message").GetString()}");
        }

        var tools = response.RootElement.GetProperty("result").GetProperty("tools");
        Assert.NotEqual(JsonValueKind.Undefined, tools.ValueKind);

        var toolNames = tools.EnumerateArray().Select(t => t.GetProperty("name").GetString()).ToList();
        Assert.Contains("Echo", toolNames);
        Assert.Contains("ReverseEcho", toolNames);
        Assert.Contains("SayHello", toolNames);

        _output.WriteLine($"All registered tools: {tools}");
    }
}
