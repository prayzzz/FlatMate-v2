using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration
{
    public class Migrator
    {
        private const string MigrationsTableName = "Migrations";

        private readonly MigrationSettings _settings;
        private readonly ILogger _logger;

        public Migrator(ILoggerFactory loggerFactory, MigrationSettings settings)
        {
            _settings = settings;
            _logger = loggerFactory.CreateLogger<Migrator>();
        }

        private SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_settings.ConnectionString);
            connection.Open();

            return connection;
        }

        private void EnsureMigrationTable(SqlConnection connection)
        {
            if (!string.IsNullOrEmpty(_settings.Schema) && !IsSchemaAvailable(connection))
            {
                CreateSchema(connection);
            }

            if (!IsTableAvailable(connection))
            {
                CreateTable(connection);
            }
        }

        private void CreateTable(SqlConnection connection)
        {
            throw new System.NotImplementedException();
        }

        private void CreateSchema(SqlConnection connection)
        {
            _logger.LogDebug($"Creating schema '{_settings.Schema}'");

            var query = $"CREATE SCHEMA [{_settings.Schema}]";
            using (var command = new SqlCommand(query, connection))
            {
                ExecuteNonQuery(command);
            }
        }

        private bool IsTableAvailable(SqlConnection connection)
        {
            _logger.LogDebug($"Checking for table '{_settings.Schema}.{MigrationsTableName}'");

            var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{_settings.Schema}' AND TABLE_NAME = '{MigrationsTableName}'";
            using (var command = new SqlCommand(query, connection))
            {
                return ExecuteScalar(command) != null;
            }
        }

        private bool IsSchemaAvailable(SqlConnection connection)
        {
            _logger.LogDebug($"Checking for schema '{_settings.Schema}'");

            var query = $"SELECT name FROM sys.schemas WHERE [name] = '{_settings.Schema}'";
            using (var command = new SqlCommand(query, connection))
            {
                return ExecuteScalar(command) != null;
            }
        }

        private object ExecuteScalar(IDbCommand sqlCommand)
        {
            _logger.LogDebug($"Executing  query '{sqlCommand.CommandText}'");
            return sqlCommand.ExecuteScalar();
        }

        private int ExecuteNonQuery(IDbCommand sqlCommand)
        {
            _logger.LogDebug($"Executing query '{sqlCommand.CommandText}'");
            return sqlCommand.ExecuteNonQuery();
        }


        //public int Execute(IEnumerable<string> arguments, Settings settings)
        //{
        //    if (!MsSqlDatabase.IsDbScriptsTableAvailable(settings))
        //    {
        //        return 1;
        //    }

        //    var scriptFilePaths = ShowMissingScriptsCommand.GetMissingScripts(settings).ToList();

        //    if (!scriptFilePaths.Any())
        //    {
        //        _console.WriteLine("No missing scripts");
        //        return 0;
        //    }

        //    using (var connection = new SqlConnection(MsSqlDatabase.GetConnectionString(settings)))
        //    {
        //        connection.Open();

        //        foreach (var filePath in scriptFilePaths)
        //        {
        //            var transaction = connection.BeginTransaction();

        //            _console.WriteLine($" {Path.GetFileName(filePath)}");
        //            var scriptContent = File.ReadAllText(filePath);

        //            using (var command = new SqlCommand())
        //            {
        //                command.Transaction = transaction;
        //                command.Connection = connection;

        //                foreach (var sqlBatch in GoRegexPattern.Split(scriptContent))
        //                {
        //                    command.CommandText = sqlBatch;

        //                    try
        //                    {
        //                        command.ExecuteNonQuery();
        //                    }
        //                    catch (Exception)
        //                    {
        //                        transaction.Rollback();
        //                        throw;
        //                    }
        //                }

        //                transaction.Commit();
        //            }
        //        }
        //    }

        //    return 0;
        //}
    }
}
