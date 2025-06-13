using System.Diagnostics;
using System.Text.Json;
using Xunit.Abstractions;

namespace AteraMcp.IntegrationTests;

    public class TestProcess : IDisposable
    {
        private readonly Process _process;
        private readonly ITestOutputHelper _output;

        public TestProcess(string exePath, ITestOutputHelper output)
        {
            _output = output;
            _output?.WriteLine($"Creating process for: {exePath}");

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                }
            };
        }

        public void Start()
        {
            _output?.WriteLine($"Starting process: {_process.StartInfo.FileName}");
            _output?.WriteLine($"Working directory: {_process.StartInfo.WorkingDirectory}");

            try
            {
                if (!_process.Start())
                {
                    throw new InvalidOperationException("Failed to start process");
                }

                // Handle error output
                _process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        _output?.WriteLine($"ERROR: {args.Data}");
                    }
                };
                _process.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                _output?.WriteLine($"Process start failed: {ex}");
                throw;
            }
        }

    public JsonDocument SendRequest(object request)
    {
        var json = JsonSerializer.Serialize(request);
        _output?.WriteLine($"Sending: {json}");
        
        _process.StandardInput.WriteLine(json);
        _process.StandardInput.Flush();

        string? response = null;
        while (true)
        {
            response = _process.StandardOutput.ReadLine();
            if (response == null)
            {
                throw new InvalidOperationException("Process closed without response");
            }

            _output?.WriteLine($"Read: {response}");

            // Skip any non-JSON responses
            try
            {
                if (response.StartsWith("{") || response.StartsWith("["))
                {
                    return JsonDocument.Parse(response);
                }
            }
            catch (JsonException)
            {
                // Not valid JSON, keep reading
                continue;
            }
        }
    }

    public void Dispose()
    {
        if (!_process.HasExited)
        {
            _process.Kill();
        }
        _process.Dispose();
    }
}
