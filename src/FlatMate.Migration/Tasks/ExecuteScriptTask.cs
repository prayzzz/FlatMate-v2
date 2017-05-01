using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using FlatMate.Migration.Common;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.Tasks
{
    public class ExecuteScriptTask
    {
        private static readonly Regex GoRegexPattern = new Regex("^GO", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly SqlCommandExecutor _commandExecutor;
        private readonly ILogger _logger;

        public ExecuteScriptTask(ILoggerFactory loggerFactory, SqlCommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public void ExecuteScript(string filePath, IDbConnection connection)
        {
            _logger.LogInformation($"Executing script {Path.GetFileName(filePath)}");

            var scriptContent = File.ReadAllText(filePath);
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;

                    foreach (var sqlBatch in GoRegexPattern.Split(scriptContent))
                    {
                        command.CommandText = sqlBatch;

                        try
                        {
                            _commandExecutor.ExecuteNonQuery(command);
                        }
                        catch (Exception)
                        {
                            // if one command fails, all commands will be rolled back
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                // only executed if all commands succeed
                transaction.Commit();
            }
        }
    }
}