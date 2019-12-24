using OKN.WebApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Psprto.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureHostConfiguration(config =>
                {
                    // Uses DOTNET_ environment variables and command line args
                }).ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                        optional: true);
                    config.AddUserSecrets<Startup>();
                    config.AddEnvironmentVariables();
                    // JSON files, User secrets, environment variables and command line arguments
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Adds loggers for console, debug, event source, and EventLog (Windows only)
                });
    }
}