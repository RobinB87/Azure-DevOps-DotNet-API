using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace AzureDevOpsAPI
{
    /// <summary>
    /// Class to initialize services
    /// </summary>
    public static class Init
    {
        /// <summary>
        /// Configure the services
        /// </summary>
        /// <returns></returns>
        public static IServiceProvider ConfigureServices()
        {
            ConfigureLogger();

            var builder = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json");
                })
                .ConfigureServices((host, services) =>
                {
                    services.AddAzureDevOps(host.Configuration.GetSection("AzureDevOps"));
                })
                .UseSerilog()
                .UseConsoleLifetime();

            return builder.Build().Services;
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                // Only log from System namespace when loglevel is warning or higher.
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Debug()
                .CreateLogger();
        }
    }
}
