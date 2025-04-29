using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SpotifyMcpServer
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs/server.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddMcpServer()
                        .WithStdioServerTransport()
                        .WithToolsFromAssembly();
                });
            await builder.Build().RunAsync();
            return 0;
        }
    }
}
