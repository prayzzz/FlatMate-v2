using System.Data;
using System.Reflection;
using FlatMate.Migration.Common;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.Tasks
{
    public class TableTask
    {
        private readonly SqlCommandExecutor _commandExecutor;
        private readonly ILogger _logger;
        private readonly ResourceLoader _resourceLoader;

        public TableTask(ILoggerFactory loggerFactory, ResourceLoader resourceLoader, SqlCommandExecutor commandExecutor)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _commandExecutor = commandExecutor;
            _resourceLoader = resourceLoader;
        }

        public void CreateTable(IDbConnection connection, MigrationSettings settings)
        {
            _logger.LogInformation($"Creating migration table {settings.DbTableEscaped}");

            var createTable = _resourceLoader.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "Resources.MigrationsTable.sql");
            createTable = createTable.Replace("##SCRIPTTABLE##", settings.DbSchemaAndTableEscaped);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = createTable;
                _commandExecutor.ExecuteNonQuery(command);
            }
        }

        public bool IsTableAvailable(IDbConnection connection, MigrationSettings settings)
        {
            _logger.LogInformation($"Checking for migration table {settings.DbSchemaAndTableEscaped}");

            string selectTable;
            if (settings.IsSchemaSet)
            {
                selectTable = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{settings.Schema}' AND TABLE_NAME = '{settings.Table}'";
            }
            else
            {
                selectTable = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{settings.Table}'";
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = selectTable;
                return _commandExecutor.ExecuteScalar(command) != null;
            }
        }
    }
}