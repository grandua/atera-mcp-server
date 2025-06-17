using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace AteraMcp.IntegrationTests;

/// <summary>
/// This class tests temprary MCP server debugging tools that will be removed in a final release.
/// </summary>
[Trait("Category", "Integration")]
public class AteraMcpServerFacts : IDisposable
{
    private readonly string _serverPath;
    private readonly ITestOutputHelper _output;

    public AteraMcpServerFacts(ITestOutputHelper output)
    {
        _output = output;
        var testDir = Path.GetDirectoryName(typeof(AteraMcpServerFacts).Assembly.Location);
        // Robustly resolve the build configuration and platform using solution root and project output
        var testAssemblyDir = Path.GetDirectoryName(typeof(AteraMcpServerFacts).Assembly.Location)!;
        var assemblyDirInfo = new DirectoryInfo(testAssemblyDir);
        var tfmName = assemblyDirInfo.Name; // e.g. net9.0
        var configName = assemblyDirInfo.Parent!.Name; // e.g. Debug or Release
        var solutionRoot = Path.GetFullPath(Path.Combine(testAssemblyDir, "../../../../"));
        var exeName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "AteraMcp.exe" : "AteraMcp";
        var ateraMcpExePath = Path.Combine(solutionRoot, "AteraMcp", "bin", configName, tfmName, exeName);

        _serverPath = ateraMcpExePath;
        Console.WriteLine($"EXE PATH: {_serverPath}");
        if (!File.Exists(_serverPath))
        {
            throw new FileNotFoundException(
                $"Could not find AteraMcp executable at: {_serverPath}",
                $"Checked from: {Directory.GetCurrentDirectory()}"
            );
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
