using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using prayzzz.Common.Results;

namespace FlatMate.Migration
{
    public class Migrator
    {
        private static readonly Regex GoRegexPattern = new Regex("^GO", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly MigrationSettings _settings;

        public Migrator(MigrationSettings settings)
        {
            _settings = settings;
        }

        public Result Run()
        {
            Console.WriteLine("Running Migrations...");

            using (var connection = GetConnection())
            {
                EnsureMigrationTable(connection);

                var missingScripts = GetMissingScripts(connection);
                ExecuteMissingScripts(connection, missingScripts);
            }

            Console.WriteLine();
            return SuccessResult.Default;
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

        private void ExecuteMissingScripts(SqlConnection connection, IEnumerable<string> scriptFilePaths)
        {
            foreach (var filePath in scriptFilePaths)
            {
                var transaction = connection.BeginTransaction();

                Console.WriteLine($" {Path.GetFileName(filePath)}");
                var scriptContent = File.ReadAllText(filePath);

                using (var command = new SqlCommand())
                {
                    command.Transaction = transaction;
                    command.Connection = connection;

                    foreach (var sqlBatch in GoRegexPattern.Split(scriptContent))
                    {
                        command.CommandText = sqlBatch;

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }

                    transaction.Commit();
                }
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

        private IEnumerable<string> GetMissingScripts(SqlConnection connection)
        {
            var dbScripts = new List<string>();
            var localScripts = Directory.GetFiles(Path.GetFullPath(_settings.MigrationsFolder), "*.sql")
                                        .OrderBy(x => x)
                                        .ToList();

            using (var sqlConnection = new SqlConnection(_settings.ConnectionString))
            {
                sqlConnection.Open();

                using (var command = new SqlCommand($"SELECT * FROM {_settings.DbSchemaAndTableEscaped}", sqlConnection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dbScripts.Add(reader.GetString(1));
                    }
                }
            }

            if (!dbScripts.Any())
            {
                return localScripts;
            }

            var missingScripts = new List<string>();

            foreach (var localScriptName in localScripts)
            {
                if (dbScripts.All(name => name != Path.GetFileNameWithoutExtension(localScriptName)))
                {
                    missingScripts.Add(localScriptName);
                }
            }

            return missingScripts;
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

        //        foreach (var filePath in scriptFilePaths)
        //        connection.Open();

        //    using (var connection = new SqlConnection(MsSqlDatabase.GetConnectionString(settings)))
        //    }
        //        return 0;
        //        Console.WriteLine("No missing scripts");
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
        //        {
        //            var transaction = connection.BeginTransaction();

        //            Console.WriteLine($" {Path.GetFileName(filePath)}");
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