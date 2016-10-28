using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;
using static UniversalDbUpdater.MySql.MySqlDatabase;

namespace UniversalDbUpdater.MySql.Commands
{
    public class ShowMissingScriptsCommand : ICommand
    {
        private readonly IConsoleFacade _console;

        public ShowMissingScriptsCommand(IConsoleFacade console)
        {
            _console = console;
        }

        public CommandType CommandType => CommandType.MySql;

        public string[] CommandName => new[] { "s", "show" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Showing missing scripts...");
            _console.WriteLine();

            if (!IsDbScriptsTableAvailable(settings))
            {
                _console.WriteLine($"Table '{GetTableName(settings.Schema, settings.Table)}' doesn't exist");
                return 1;
            }

            var missingScripts = GetMissingScripts(settings).ToList();

            if (!missingScripts.Any())
            {
                _console.WriteLine("No missing scripts");
                return 0;
            }

            _console.WriteLine($" {missingScripts.Count} missing scripts");

            foreach (var missingScript in missingScripts)
            {
                _console.WriteLine($" {Path.GetFileName(missingScript)}");
            }

            return 0;
        }

        public static IEnumerable<string> GetMissingScripts(Settings settings)
        {
            var localScripts = Directory.GetFiles(Path.GetFullPath(settings.ScriptsDirectory), "*.mysql").ToList();
            var dbScripts = new List<string>();

            using (var sqlConnection = new MySqlConnection(GetConnectionString(settings)))
            {
                sqlConnection.Open();

                using (var command = new MySqlCommand($"SELECT * FROM `{GetTableName(settings.Schema, settings.Table)}`", sqlConnection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dbScripts.Add(reader.GetString(1));
                    }
                }
            }

            if (!dbScripts.Any())
            {
                return localScripts;
            }

            var missingScripts = new List<string>();

            foreach (var localScriptName in localScripts)
            {
                if (dbScripts.All(name => name != Path.GetFileNameWithoutExtension(localScriptName)))
                {
                    missingScripts.Add(localScriptName);
                }
            }

            return missingScripts;
        }
    }
}