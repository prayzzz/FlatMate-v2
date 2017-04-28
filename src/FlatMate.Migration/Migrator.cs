using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace FlatMate.Migration
{
    public class Migrator
    {
        private readonly MigrationSettings _settings;

        public Migrator(MigrationSettings settings)
        {
            _settings = settings;
        }

        public void Run()
        {
            Console.WriteLine("Running Migrations...");

            using (var connection = GetConnection())
            {
                EnsureMigrationTable(connection);
            }

            Console.WriteLine();
        }

        private void CreateSchema(SqlConnection connection)
        {
            WriteInfo($"Creating schema {_settings.DbSchemaEscaped}");

            var query = $"CREATE SCHEMA {_settings.DbSchemaEscaped}";
            using (var command = new SqlCommand(query, connection))
            {
                ExecuteNonQuery(command);
            }
        }

        private void CreateTable(SqlConnection connection)
        {
            WriteInfo($"Creating migration table {_settings.DbTableEscaped}");

            var assemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;
            var templateFilePath = $"{assemblyName}.Resources.MigrationsTable.sql";
            var script = ResourceHelper.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, templateFilePath);
            script = script.Replace("##SCRIPTTABLE##", _settings.DbSchemaAndTableEscaped);

            if (string.IsNullOrEmpty(script))
            {
                throw new FileNotFoundException(templateFilePath);
            }

            using (var command = new SqlCommand(script, connection))
            {
                ExecuteNonQuery(command);
            }
        }

        private void EnsureMigrationTable(SqlConnection connection)
        {
            if (_settings.IsSchemaSet && !IsSchemaAvailable(connection))
            {
                if (_settings.CreateMissingSchema)
                {
                    CreateSchema(connection);
                }
                else
                {
                    throw new Exception($"Missing database schema for migration table {_settings.DbSchemaEscaped}.");
                }
            }

            if (!IsTableAvailable(connection))
            {
                CreateTable(connection);
            }
        }

        private int ExecuteNonQuery(IDbCommand sqlCommand)
        {
            WriteDebug($"Executing query '{sqlCommand.CommandText}'");
            return sqlCommand.ExecuteNonQuery();
        }

        private object ExecuteScalar(IDbCommand sqlCommand)
        {
            WriteDebug($"Executing  query '{sqlCommand.CommandText}'");
            return sqlCommand.ExecuteScalar();
        }

        private SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_settings.ConnectionString);
            connection.Open();

            return connection;
        }

        private bool IsSchemaAvailable(SqlConnection connection)
        {
            WriteInfo($"Checking for schema {_settings.DbSchemaEscaped}");

            var query = $"SELECT name FROM sys.schemas WHERE [name] = '{_settings.Schema}'";
            using (var command = new SqlCommand(query, connection))
            {
                return ExecuteScalar(command) != null;
            }
        }

        private bool IsTableAvailable(SqlConnection connection)
        {
            WriteInfo($"Checking for table {_settings.DbSchemaAndTableEscaped}");

            var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{_settings.Table}'";
            if (_settings.IsSchemaSet)
            {
                query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{_settings.Schema}' AND TABLE_NAME = '{_settings.Table}'";
            }

            using (var command = new SqlCommand(query, connection))
            {
                return ExecuteScalar(command) != null;
            }
        }

        private void WriteDebug(string message)
        {
            if (_settings.LogDebug)
            {
                Console.WriteLine("debug: " + message);
            }
        }

        private void WriteInfo(string message)
        {
            Console.WriteLine("info: " + message);
        }

        //    using (var connection = new SqlConnection(MsSqlDatabase.GetConnectionString(settings)))
        //    }
        //        return 0;
        //        _console.WriteLine("No missing scripts");
        //    {

        //    if (!scriptFilePaths.Any())

        //    var scriptFilePaths = ShowMissingScriptsCommand.GetMissingScripts(settings).ToList();
        //    }
        //        return 1;
        //    {
        //    if (!MsSqlDatabase.IsDbScriptsTableAvailable(settings))
        //{

        //public int Execute(IEnumerable<string> arguments, Settings settings)
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