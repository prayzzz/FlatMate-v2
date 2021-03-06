﻿using System;
using System.Data;
using System.Data.SqlClient;
using FlatMate.Migration.Common;
using FlatMate.Migration.Tasks;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Migration
{
    public class Migrator
    {
        private readonly SqlCommandExecutor _commandExecutor;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ResourceLoader _resourceLoader;
        private readonly MigrationSettings _settings;

        public Migrator(ILoggerFactory loggerFactory, MigrationSettings settings)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;

            _commandExecutor = new SqlCommandExecutor(loggerFactory);
            _resourceLoader = new ResourceLoader(_loggerFactory);
        }

        public Result Run()
        {
            var missingTask = new MissingScriptTask(_loggerFactory, _commandExecutor);
            var executeTask = new ExecuteScriptTask(_loggerFactory, _commandExecutor);

            using (var connection = GetConnection())
            {
                EnsureMigrationStructure(connection);

                foreach (var script in missingTask.GetMissingScripts(connection, _settings))
                {
                    executeTask.ExecuteScript(script, connection);
                }
            }

            return Result.Success;
        }

        private void EnsureMigrationStructure(IDbConnection connection)
        {
            var schemaTask = new SchemaTask(_loggerFactory, _commandExecutor);
            if (_settings.IsSchemaSet && !schemaTask.IsSchemaAvailable(connection, _settings))
            {
                if (_settings.CreateMissingSchema)
                {
                    schemaTask.CreateSchema(connection, _settings);
                }
                else
                {
                    throw new Exception($"Missing database schema for migration table {_settings.DbSchemaEscaped}.");
                }
            }

            var tableTask = new TableTask(_loggerFactory, _resourceLoader, _commandExecutor);
            if (!tableTask.IsTableAvailable(connection, _settings))
            {
                tableTask.CreateTable(connection, _settings);
            }
        }

        private SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_settings.ConnectionString);
            connection.Open();

            return connection;
        }
    }
}