using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration
{
    public class Migrator
    {
        private readonly MigrationSettings _settings;
        private readonly ILogger _logger;

        public Migrator(ILoggerFactory loggerFactory, MigrationSettings settings)
        {
            _settings = settings;
            _logger = loggerFactory.CreateLogger<Migrator>();
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_settings.ConnectionString);
            connection.Open();

            return connection;
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
