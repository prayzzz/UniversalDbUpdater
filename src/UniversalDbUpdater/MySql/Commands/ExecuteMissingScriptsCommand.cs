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

        public CommandType CommandType => CommandType.MySql;

        public string[] CommandName => new[] { "e", "execute" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Executing missing scripts...");
            _console.WriteLine();

            if (!MySqlDatabase.IsDbScriptsTableAvailable(settings))
            {
                return 1;
            }

            var scriptFilePaths = ShowMissingScriptsCommand.GetMissingScripts(settings).ToList();

            if (!scriptFilePaths.Any())
            {
                _console.WriteLine("No missing scripts");
                return 0;
            }

            using (var connection = new MySqlConnection(MySqlDatabase.GetConnectionString(settings)))
            {
                connection.Open();

                foreach (var filePath in scriptFilePaths)
                {
                    var script = new MySqlScript(connection);
                    var transaction = script.Connection.BeginTransaction();

                    _console.WriteLine($" {Path.GetFileName(filePath)}");
                    var scriptContent = File.ReadAllText(filePath);

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
    }
}