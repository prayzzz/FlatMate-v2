using FlatMate.Web.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlatMate.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var hostConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "urls", "http://localhost:5000" } })
                                                       .AddCommandLine(args)
                                                       .Build();

            new WebHostBuilder().UseKestrel()
                                .UseConfiguration(hostConfig)
                                .UseContentRoot(Directory.GetCurrentDirectory())
                                .ConfigureAppConfiguration((context, builder) => ConfigureAppConfiguration(args, context, builder))
                                .ConfigureLogging(ConfigureLogging)
                                .UseStartup<Startup>()
                                .Build()
                                .Run();
        }

        private static void ConfigureAppConfiguration(IReadOnlyList<string> args, WebHostBuilderContext context, IConfigurationBuilder builder)
        {
            var env = context.HostingEnvironment;

            builder.AddJsonFile("appsettings.json", true, true)
                   .AddJsonFile($"appsettings.{env.EnvironmentName.ToLower()}.json", true, true);

            if (args.Any() && !string.IsNullOrEmpty(args[0]))
            {
                builder.AddJsonFile(args[0], true);
            }

            builder.AddEnvironmentVariables("flatmate_")
                   .BuildConnectionString();
        }

        private static void ConfigureLogging(WebHostBuilderContext context, ILoggingBuilder builder)
        {
            // Enable all logs
            builder.SetMinimumLevel(LogLevel.Trace);

            var logger = new LoggerConfiguration().ReadFrom.Configuration(context.Configuration)
                                                  .CreateLogger();

            builder.AddSerilog(logger);
        }
    }
}