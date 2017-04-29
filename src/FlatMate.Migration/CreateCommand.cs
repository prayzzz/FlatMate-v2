using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using prayzzz.Common.Results;

namespace FlatMate.Migration
{
    public class CreateCommand : ICommand
    {
        private readonly MigrationSettings _settings;

        public CreateCommand(MigrationSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<string> CommandNames => new[] { "c", "create" };

        public Result Execute(IEnumerable<string> arguments)
        {
            Console.WriteLine("Creating new script...");
            Console.WriteLine();

            var scriptName = string.Empty;
            while (string.IsNullOrEmpty(scriptName))
            {
                Console.WriteLine("Script name:");
                scriptName = Console.ReadLine();
                Console.WriteLine();
            }

            
            var assemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;
            var templateFilePath = $"{assemblyName}.Resources.ScriptTemplate.sql";
            var script = ResourceHelper.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, templateFilePath);

            if (string.IsNullOrEmpty(script))
            {
                return new ErrorResult(ErrorType.InternalError, "Script Template not found");
            }

            var scriptFileName = DateTime.Now.ToString(Constants.DateFormat) + "_" + scriptName.Replace(" ", "_");
            script = script.Replace("##SCRIPTTABLE##", _settings.DbSchemaAndTableEscaped);
            script = script.Replace("##FILENAME##", scriptFileName);

            var scriptFile = Path.GetFullPath(Path.Combine(_settings.MigrationsFolder, scriptFileName + ".sql"));
            File.WriteAllText(scriptFile, script);

            Console.WriteLine();
            Console.WriteLine($"Script created: {scriptFile}");

            return new SuccessResult();
        }
    }
}