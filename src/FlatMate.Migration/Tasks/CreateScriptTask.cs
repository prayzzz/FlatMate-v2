using System;
using System.IO;
using System.Reflection;
using FlatMate.Migration.Common;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.Tasks
{
    public class CreateScriptTask
    {
        private readonly ILogger _logger;
        private readonly ResourceLoader _resourceLoader;

        public CreateScriptTask(ILoggerFactory loggerFactory, ResourceLoader resourceLoader)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _resourceLoader = resourceLoader;
        }

        public void CreateScript(string scriptName, MigrationSettings settings)
        {
            _logger.LogDebug($"Creating script {scriptName}");

            var script = _resourceLoader.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "Resources.ScriptTemplate.sql");

            scriptName = DateTime.Now.ToString(Constants.DateFormat) + "_" + scriptName.Replace(" ", "_");
            script = script.Replace("##SCRIPTTABLE##", settings.DbSchemaAndTableEscaped);
            script = script.Replace("##FILENAME##", scriptName);

            var scriptFile = Path.GetFullPath(Path.Combine(settings.MigrationsFolder, scriptName + ".sql"));
            File.WriteAllText(scriptFile, script);
        }
    }
}