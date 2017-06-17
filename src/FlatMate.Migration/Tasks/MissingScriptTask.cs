using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using FlatMate.Migration.Common;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.Tasks
{
    public class MissingScriptTask
    {
        private readonly SqlCommandExecutor _commandExecutor;
        private readonly ILogger _logger;

        public MissingScriptTask(ILoggerFactory loggerFactory, SqlCommandExecutor commandExecutor)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _commandExecutor = commandExecutor;
        }

        public IEnumerable<string> GetMissingScripts(IDbConnection connection, MigrationSettings settings)
        {
            _logger.LogDebug("Looking for missing scripts");

            var migrationFolderPath = Path.GetFullPath(settings.MigrationsFolder);
            if (!Directory.Exists(migrationFolderPath))
            {
                _logger.LogError($"Migrations folder {migrationFolderPath} not found");
                return Enumerable.Empty<string>();
            }

            var dbScripts = new List<string>();
            var localScripts = Directory.GetFiles(migrationFolderPath, "*.sql")
                                        .OrderBy(x => x)
                                        .ToList();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {settings.DbSchemaAndTableEscaped}";
                using (var reader = _commandExecutor.ExecuteReader(command))
                {
                    while (reader.Read())
                    {
                        dbScripts.Add(reader.GetString(1));
                    }
                }
            }

            var missingScripts = new List<string>();
            foreach (var localScriptName in localScripts)
            {
                if (dbScripts.All(name => name != Path.GetFileNameWithoutExtension(localScriptName)))
                {
                    missingScripts.Add(localScriptName);
                }
            }

            _logger.LogInformation($"{missingScripts.Count} missing scripts found");
            return missingScripts;
        }
    }
}