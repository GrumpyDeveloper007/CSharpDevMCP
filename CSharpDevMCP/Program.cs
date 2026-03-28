using CSharpDevMCP;
using CSharpDevMCP.FlaUI;
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
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Error;
});
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new Implementation { Name = "TimeServer", Version = "1.0.0" };
        //options.ListenAnyIP(5001, listenOptions => // Listen on port 5001 for HTTPS
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services.AddSingleton<SnapshotBuilder, SnapshotBuilder>();
builder.Services.AddSingleton<ElementRegistry, ElementRegistry>();
builder.Services.AddSingleton<SessionManager, SessionManager>();

var server = builder.Build();

if (Debugger.IsAttached)
{
    var test3 = new SessionManager();
    var windows = test3.ListWindows();
    var (handle, title, processName) = windows.SingleOrDefault(o => o.title == StaticSettings.SettingValues.ApplicationName);
    var snapshot = new SnapshotBuilder(new ElementRegistry());
    var window = test3.GetWindow(handle);
    if (window == null)
    {
        Console.WriteLine($"Could not find window with title {StaticSettings.SettingValues.ApplicationName}");
        return;
    }
    var snapshotText = snapshot.BuildSnapshot(handle, window);

    var test = new GitToolMCP();
    var test2 = test.GetPendingChanges();
}
else
{
    await server.RunAsync();
}


