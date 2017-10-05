using System.Collections.Generic;
using FlatMate.Migration.Common;
using FlatMate.Migration.DotNet.Commands;
using Microsoft.Extensions.Logging;

namespace FlatMate.Migration.DotNet
{
    public class CommandLibrary
    {
        private readonly Dictionary<string, ICommand> _nameToCommand;

        public CommandLibrary(ILoggerFactory loggerFactory, MigrationSettings settings)
        {
            _nameToCommand = new Dictionary<string, ICommand>();

            AddCommand(new CreateCommand(loggerFactory, settings));
            AddCommand(new InitCommand(loggerFactory, settings));
        }

        public ICommand Get(string parameter)
        {
            return _nameToCommand.TryGetValue(parameter, out var command) ? command : null;
        }

        private void AddCommand(ICommand command)
        {
            foreach (var commandName in command.CommandNames)
            {
                _nameToCommand.Add(commandName, command);
            }
        }
    }
}