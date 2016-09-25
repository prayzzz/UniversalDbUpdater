using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Commands
{
    public class ShowMissingScriptsCommand : ICommand
    {
        private static ShowMissingScriptsCommand _instance;

        private ShowMissingScriptsCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new ShowMissingScriptsCommand());

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] Command => new[] { "-s", "--show" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            Console.WriteLine("Showing missing scripts...");
            Console.WriteLine();

            if (!Database.IsDbScriptsTableAvailable(settings))
            {
                return 1;
            }

            var missingScripts = GetMissingScripts(settings).ToList();

            if (!missingScripts.Any())
            {
                Console.WriteLine("No missing scripts");
                return 0;
            }

            Console.WriteLine("{0} missing scripts", missingScripts.Count);
            Console.WriteLine();

            foreach (var missingScript in missingScripts)
            {
                Console.WriteLine("{0}", missingScript);
            }

            return 0;
        }

        public static IEnumerable<string> GetMissingScripts(Settings settings)
        {
            var localScripts = Directory.GetFiles(".", "*.mysql").Select(Path.GetFileName).ToList();
            var dbScripts = new List<DbScript>();

            using (var sqlConnection = new MySqlConnection(Database.GetConnectionString(settings)))
            {
                sqlConnection.Open();

                using (var command = new MySqlCommand("SELECT * FROM `infrastructure.dbscripts`", sqlConnection))
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

        public void HelpShort()
        {
            Console.WriteLine(" -s --show \t Shows scripts missing in database");
        }
    }
}