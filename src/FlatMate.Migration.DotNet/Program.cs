using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using prayzzz.Common.Results;

namespace FlatMate.Migration.DotNet
{
    public class Program
    {
        private static CommandLibrary _commandLibrary;

        private static readonly Dictionary<string, string> SwitchMappings = new Dictionary<string, string>
        {
            { "--backup", "Settings:BackupDirectory" },

            { "--connection", "Settings:ConnectionString" },

            { "--schema", "Settings:Schema" },

            { "--migrations", "Settings:MigrationsFolder" },

            { "--table", "Settings:Table" },
        };


        public static void Main(string[] args)
        {
            var loadSettings = LoadSettings(args);
            if (loadSettings.IsError)
            {
                Exit(loadSettings);
            }

            var settings = loadSettings.Data;
            _commandLibrary = new CommandLibrary(settings);

            var checkSettings = CheckSettings(settings);
            if (checkSettings.IsError)
            {
                Exit(checkSettings);
            }

            var evaluateArguments = EvaluateArguments(args);
            Exit(evaluateArguments);
        }

        private static Result CheckSettings(MigrationSettings settings)
        {
            if (!Directory.Exists(settings.MigrationsFolder))
            {
                return new ErrorResult(ErrorType.NotFound, $"Directory '{settings.MigrationsFolder}' doesn't exist");
            }

            return SuccessResult.Default;
        }

        private static Result EvaluateArguments(IReadOnlyList<string> args)
        {
            if (args.Count < 1)
            {
                return new ErrorResult(ErrorType.ValidationError, "No command");
            }

            var commandName = args[0];
            var command = _commandLibrary.Get(commandName);
            if (command == null)
            {
                return new ErrorResult(ErrorType.ValidationError, $"Unknown command: {commandName}");
            }

            return command.Execute(args.Skip(1));
        }

        private static void Exit(Result result)
        {
            if (result.IsError)
            {
                Console.WriteLine($"error: {result.ToMessageString()}");
                Environment.Exit(1);
            }

            Environment.Exit(0);
        }

        private static Result<MigrationSettings> LoadSettings(IReadOnlyList<string> args)
        {
            // remove command from args
            var strippedArgs = args.ToArray();
            if (args.Count > 0 && !args[0].StartsWith("-") && !args[0].StartsWith("--"))
            {
                strippedArgs = strippedArgs.Skip(1).ToArray();
            }

            var configuration = new ConfigurationBuilder().AddCommandLine(strippedArgs, SwitchMappings)
                                                          .Build();

            var settings = configuration.GetSection("Settings")
                                        .Get<MigrationSettings>();

            return new SuccessResult<MigrationSettings>(settings);
        }
    }
}
