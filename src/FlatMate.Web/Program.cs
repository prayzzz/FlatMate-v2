using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FlatMate.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                          .AddJsonFile("hosting.json", optional: false)
                                                          .Build();

            var host = new WebHostBuilder().UseKestrel()
                                           .UseConfiguration(configuration)
                                           .UseContentRoot(Directory.GetCurrentDirectory())
                                           .UseStartup<Startup>()
                                           .Build();

            host.Run();
        }
    }
}