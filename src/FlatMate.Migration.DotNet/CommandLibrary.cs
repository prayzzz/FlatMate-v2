using System.Collections.Generic;

namespace FlatMate.Migration.DotNet
{
    public class CommandLibrary
    {
        private readonly Dictionary<string, ICommand> _nameToCommand;

        public CommandLibrary(MigrationSettings settings)
        {
            _nameToCommand = new Dictionary<string, ICommand>();

            AddCommand(new CreateCommand(settings));
        }

        public ICommand Get(string parameter)
        {
            return _nameToCommand.TryGetValue(parameter, out ICommand command) ? command : null;
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