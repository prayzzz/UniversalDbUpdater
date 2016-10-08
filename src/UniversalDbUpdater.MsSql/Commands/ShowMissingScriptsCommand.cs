using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Commands
{
    public class ShowMissingScriptsCommand : ICommand
    {
        private readonly IConsoleFacade _console;

        public ShowMissingScriptsCommand(IConsoleFacade console)
        {
            _console = console;
        }

        public CommandType CommandType => CommandType.MsSql;

        public string[] CommandName => new[] { "s", "show" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Showing missing scripts...");
            _console.WriteLine();

            if (!Database.IsDbScriptsTableAvailable(settings))
            {
                return 1;
            }

            var missingScripts = GetMissingScripts(settings).ToList();

            if (!missingScripts.Any())
            {
                _console.WriteLine("No missing scripts");
                return 0;
            }


            _console.WriteLine($"{missingScripts.Count} missing scripts");
            _console.WriteLine();

            foreach (var missingScript in missingScripts)
            {
                _console.WriteLine($"{missingScript}");
            }

            return 0;
        }

        public static IEnumerable<string> GetMissingScripts(Settings settings)
        {
            var localScripts = Directory.GetFiles(settings.ScriptsDirectory, "*.sql").ToList();
            var dbScripts = new List<DbScript>();

            using (var sqlConnection = new SqlConnection(Database.GetConnectionString(settings)))
            {
                sqlConnection.Open();

                using (var command = new SqlCommand("SELECT * FROM [Infrastructure].[DbScripts]", sqlConnection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var script = new DbScript();
                        script.Date = reader.GetDateTime(1);
                        script.Name = reader.GetString(2);

                        dbScripts.Add(script);
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
                if (dbScripts.All(x => x.FileNameWithoutExtension != Path.GetFileNameWithoutExtension(localScriptName)))
                {
                    missingScripts.Add(localScriptName);
                }
            }

            return missingScripts;
        }
    }
}