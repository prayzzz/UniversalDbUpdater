using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Commands
{
    public class ExecuteMissingScriptsCommand : ICommand
    {
        private static ExecuteMissingScriptsCommand _instance;

        private ExecuteMissingScriptsCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new ExecuteMissingScriptsCommand());

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] Command => new[] { "-e", "--execute" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            Console.WriteLine("Executing missing scripts...");
            Console.WriteLine();

            if (!Database.IsDbScriptsTableAvailable(settings))
            {
                return 1;
            }

            var list = ShowMissingScriptsCommand.GetMissingScripts(settings).ToList();

            if (!list.Any())
            {
                Console.WriteLine("\t No missing scripts");
                return 0;
            }

            using (var connection = new MySqlConnection(Database.ConnectionString(settings)))
            {
                connection.Open();

                foreach (var scriptName in list)
                {
                    var script = new MySqlScript(connection);
                    var transaction = script.Connection.BeginTransaction();

                    Console.WriteLine(" {0}", scriptName);
                    var scriptContent = File.ReadAllText(scriptName);

                    try
                    {
                        script.Query = scriptContent;
                        script.Execute();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }

                    transaction.Commit();
                }
            }

            return 0;
        }

        public void HelpShort()
        {
            Console.WriteLine(" -e --execute \t Executes missing scripts");
        }
    }
}