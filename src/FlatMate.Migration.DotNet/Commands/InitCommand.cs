using FlatMate.Migration.Common;
using FlatMate.Migration.Tasks;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FlatMate.Migration.DotNet.Commands
{
    public class InitCommand : ICommand
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ResourceLoader _resourceLoader;
        private readonly MigrationSettings _settings;
        private readonly SqlCommandExecutor _sqlCommandExecutor;

        public InitCommand(ILoggerFactory loggerFactory, MigrationSettings settings)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
            _resourceLoader = new ResourceLoader(_loggerFactory);
            _sqlCommandExecutor = new SqlCommandExecutor(_loggerFactory);
        }

        public IEnumerable<string> CommandNames => new[] { "i", "init" };

        public Result Execute(IEnumerable<string> arguments)
        {
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                connection.Open();

                new SchemaTask(_loggerFactory, _sqlCommandExecutor).CreateSchema(connection, _settings);
                new TableTask(_loggerFactory, _resourceLoader, _sqlCommandExecutor).CreateTable(connection, _settings);
            }

            return SuccessResult.Default;
        }
    }
}
