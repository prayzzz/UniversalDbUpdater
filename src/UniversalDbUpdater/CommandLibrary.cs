using System.Collections.Generic;
using UniversalDbUpdater.Commands;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater
{
    public class CommandLibrary
    {
        private readonly Dictionary<DatabaseType, Dictionary<string, ICommand>> _commandsByType;

        public CommandLibrary()
        {
            _commandsByType = new Dictionary<DatabaseType, Dictionary<string, ICommand>>();

            DiscoverCommands();
        }

        public ICommand Get(DatabaseType type, string parameter)
        {
            var commandsByCommandName = _commandsByType[type];

            ICommand command;
            if (commandsByCommandName.TryGetValue(parameter, out command))
            {
                return command;
            }

            return null;
        }

        private void DiscoverCommands()
        {
            var console = new ConsoleFacade();
            var dateTime = new DateTimeFacade();

            AddCommand(new HelpCommand());

            AddCommand(new MySql.Commands.BackupCommand(console, dateTime));
            AddCommand(new MySql.Commands.CreateCommand(console, dateTime));
            AddCommand(new MySql.Commands.ExecuteMissingScriptsCommand(console));
            AddCommand(new MySql.Commands.InitCommand(console));
            AddCommand(new MySql.Commands.ShowMissingScriptsCommand(console));

            AddCommand(new MsSql.Commands.BackupCommand(console, dateTime));
            AddCommand(new MsSql.Commands.CreateCommand(console, dateTime));
            AddCommand(new MsSql.Commands.ExecuteMissingScriptsCommand(console));
            AddCommand(new MsSql.Commands.InitCommand(console));
            AddCommand(new MsSql.Commands.ShowMissingScriptsCommand(console));
        }

        private void AddCommand(ICommand command)
        {
            Dictionary<string, ICommand> commandsByCommandName;
            if (!_commandsByType.TryGetValue(command.DatabaseType, out commandsByCommandName))
            {
                commandsByCommandName = new Dictionary<string, ICommand>();
                _commandsByType.Add(command.DatabaseType, commandsByCommandName);
            }

            foreach (var commandName in command.CommandName)
            {
                commandsByCommandName.Add(commandName, command);
            }
        }

        public IEnumerable<ICommand> GetAll()
        {
            return new List<ICommand>();
        }
    }
}