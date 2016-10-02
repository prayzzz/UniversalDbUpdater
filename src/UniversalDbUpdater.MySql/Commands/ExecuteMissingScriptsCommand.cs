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
        private readonly IConsoleFacade _console;

        public ExecuteMissingScriptsCommand(IConsoleFacade console)
        {
            _console = console;
        }

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] CommandName => new[] { "e", "execute" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Executing missing scripts...");
            _console.WriteLine();

            if (!Database.IsDbScriptsTableAvailable(settings))
            {
                return 1;
            }

            var list = ShowMissingScriptsCommand.GetMissingScripts(settings).ToList();

            if (!list.Any())
            {
                _console.WriteLine("No missing scripts");
                return 0;
            }

            using (var connection = new MySqlConnection(Database.GetConnectionString(settings)))
            {
                connection.Open();

                foreach (var scriptName in list)
                {
                    var script = new MySqlScript(connection);
                    var transaction = script.Connection.BeginTransaction();

                    _console.WriteLine($" {scriptName}");
                    var scriptContent = File.ReadAllText(scriptName);

                    try
                    {
                        script.Query = scriptContent;
                        script.Execute();
                    }
                    catch (Exception)
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
            _console.WriteLine(" e execute \t Executes missing scripts");
        }
    }
}