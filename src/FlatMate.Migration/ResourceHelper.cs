using System.IO;
using System.Reflection;

namespace FlatMate.Migration
{
    public static class ResourceHelper
    {
        public static string GetEmbeddedFile(Assembly assembly, string fileName)
        {
             // _logger.LogDebug($"Loading DbTable template '{fileName}' from '{assembly.FullName}'");

            using (var stream = assembly.GetManifestResourceStream(fileName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return string.Empty;
        }
    }
}
