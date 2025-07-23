using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using MCP.FileSystem.Tools;

namespace MCP.FileSystem.Server;

public static class FileSystemStdioServer
{
    public static Task RunAsync(params string[] args)
    {
        return RunAsync(_ => { }, args);
    }

    public static Task RunAsync(string applicationName, string version, params string[] args)
    {
        return RunAsync(applicationName, version, _ => { }, args);
    }

    public static Task RunAsync(Action<IServiceCollection> servicesAction, params string[] args)
    {
        var assembly = Assembly.GetEntryAssembly();
        var applicationName = assembly?.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "MCP.FileSystem";
        var version = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion?.Split('+')[0] ?? "0.1.8";

        return RunAsync(applicationName, version, servicesAction, args);
    }

    public static Task RunAsync(string applicationName, string version, Action<IServiceCollection> servicesAction, params string[] args)
    {
        var builder = Host.CreateEmptyApplicationBuilder(settings: new HostApplicationBuilderSettings
        {
            ApplicationName = applicationName,
            Args = args
        });

        builder.Configuration
            .AddCommandLine(args)
            .AddEnvironmentVariables();

        // Configure logging to stderr (stdout is used for MCP protocol messages)
        builder.Services
            .AddSingleton<ILoggerFactory>(provider => 
            {
                return LoggerFactory.Create(loggingBuilder =>
                {
                    loggingBuilder.AddConsole(consoleOptions =>
                    {
                        consoleOptions.LogToStandardErrorThreshold = LogLevel.Trace;
                    });
                });
            })
            .AddMcpServer(options => options.ServerInfo = new Implementation
            {
                Name = applicationName,
                Version = version
            })
            .WithStdioServerTransport()
            .WithTools<FileSystemTool>();

        servicesAction(builder.Services);

        var host = builder.Build();

        var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            cts.Cancel();
        };

        return host.RunAsync(cts.Token);
    }
}