﻿using System.ComponentModel;
using AteraApi.DataAccess;
using ModelContextProtocol.Server;

namespace AteraMcp;

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from C#: {message}";

    [McpServerTool, Description("Echoes in reverse the message sent by the client.")] 
    public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());
}