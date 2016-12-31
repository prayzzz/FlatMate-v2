using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FlatMate.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls("http://::7432")
                .Build();

            host.Run();
        }
    }
}