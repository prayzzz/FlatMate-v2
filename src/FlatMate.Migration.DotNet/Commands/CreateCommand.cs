using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using FlatMate.Migration.Common;
using FlatMate.Migration.Tasks;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Migration.DotNet.Commands
{
    public class CreateCommand : ICommand
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ResourceLoader _resourceLoader;
        private readonly MigrationSettings _settings;

        public CreateCommand(ILoggerFactory loggerFactory, MigrationSettings settings)
        {
            _loggerFactory = loggerFactory;
            _settings = settings;
            _resourceLoader = new ResourceLoader(_loggerFactory);
        }

        public IEnumerable<string> CommandNames => new[] { "c", "create" };

        public Result Execute(IEnumerable<string> arguments)
        {
            var scriptName = string.Empty;
            while (string.IsNullOrEmpty(scriptName))
            {
                Console.WriteLine("Script name:");
                scriptName = Console.ReadLine();
                Console.WriteLine();
            }

            var task = new CreateScriptTask(_loggerFactory, _resourceLoader);
            var filePath = task.CreateScript(scriptName, _settings);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }

            return SuccessResult.Default;
        }
    }
}