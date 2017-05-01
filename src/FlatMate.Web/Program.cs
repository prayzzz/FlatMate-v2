using System.IO;
using FlatMate.Migration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlatMate.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddJsonFile("hosting.json", true)
                                                          .Build();

            var loggerFactory = new LoggerFactory().AddConsole();
            new Migrator(loggerFactory, configuration.GetSection("Migration").Get<MigrationSettings>()).Run();

            var host = new WebHostBuilder().UseKestrel()
                                           .UseConfiguration(configuration)
                                           .UseContentRoot(Directory.GetCurrentDirectory())
                                           .UseStartup<Startup>()
                                           .Build();

            host.Run();
        }
    }
}