using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.Common
{
    public class ResourceLoader
    {
        private readonly ILogger _logger;

        public ResourceLoader(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public string GetEmbeddedFile(Assembly assembly, string filePath)
        {
            _logger.LogDebug($"Loading file '{filePath}' from '{assembly.FullName}'");

            var name = $"{assembly.GetName().Name}.{filePath}";
            using (var stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException(name);
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}