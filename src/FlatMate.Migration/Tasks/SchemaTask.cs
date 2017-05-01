using System.Data;
using FlatMate.Migration.Common;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.Tasks
{
    public class SchemaTask
    {
        private readonly SqlCommandExecutor _commandExecutor;
        private readonly ILogger _logger;

        public SchemaTask(ILoggerFactory loggerFactory, SqlCommandExecutor commandExecutor)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _commandExecutor = commandExecutor;
        }

        public void CreateSchema(IDbConnection connection, MigrationSettings settings)
        {
            _logger.LogInformation($"Creating schema {settings.DbSchemaEscaped}");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"CREATE SCHEMA {settings.DbSchemaEscaped}";
                _commandExecutor.ExecuteNonQuery(command);
            }
        }

        public bool IsSchemaAvailable(IDbConnection connection, MigrationSettings settings)
        {
            _logger.LogInformation($"Checking for schema {settings.DbSchemaEscaped}");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT name FROM sys.schemas WHERE [name] = '{settings.Schema}'";
                return _commandExecutor.ExecuteScalar(command) != null;
            }
        }
    }
}