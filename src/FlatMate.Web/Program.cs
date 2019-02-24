using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FlatMate.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static void ConfigureAppConfiguration(IReadOnlyList<string> args, IConfigurationBuilder builder)
        {
            // Add JSON File passed by arguments
            if (args.Any() && !string.IsNullOrEmpty(args[0]))
            {
                builder.AddJsonFile(args[0], true);
            }
        }

        private static void ConfigureLogging(WebHostBuilderContext context, LoggerConfiguration config)
        {
            config.ReadFrom.Configuration(context.Configuration);
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .ConfigureAppConfiguration((context, builder) => ConfigureAppConfiguration(args, builder))
                          .UseStartup<Startup>()
                          .UseSerilog(ConfigureLogging)
                          .SuppressStatusMessages(true);
        }
    }
}