using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlatMate.Web.Common;
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
            WebHost.CreateDefaultBuilder()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseSerilog(ConfigureLogging)
                   .UseStartup<Startup>()
                   .ConfigureAppConfiguration((context, builder) => ConfigureAppConfiguration(args, context, builder))
                   .SuppressStatusMessages(true)
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

        private static void ConfigureLogging(WebHostBuilderContext context, LoggerConfiguration config)
        {
            config.ReadFrom.Configuration(context.Configuration);
        }
    }
}