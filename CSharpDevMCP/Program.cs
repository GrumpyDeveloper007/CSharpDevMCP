using CSharpDevMCP;
using CSharpDevMCP.MCP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using System.Diagnostics;


IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("local.settings.json")
    .AddEnvironmentVariables()
    .Build();

var settings = config.GetRequiredSection("Values").Get<SettingValues>();

if (settings == null)
{
    Console.WriteLine("Unable to load local.settings.json!");
    return;
}
StaticSettings.SettingValues = settings;

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

if (Debugger.IsAttached)
{
    var test = new GitTool();
    var test2 = test.GetPendingChanges();
}
else
{
    server.Run();
}


Console.WriteLine("Hello MCP World!");

