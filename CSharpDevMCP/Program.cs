using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Debug;
});
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new Implementation { Name = "TimeServer", Version = "1.0.0" };
        //options.ListenAnyIP(5001, listenOptions => // Listen on port 5001 for HTTPS


    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
var server = builder.Build();

server.Run();


Console.WriteLine("Hello MCP World!");

