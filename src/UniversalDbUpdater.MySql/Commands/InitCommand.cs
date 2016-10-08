using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Commands
{
    public class InitCommand : ICommand
    {
        private readonly IConsoleFacade _console;

        public InitCommand(IConsoleFacade console)
        {
            _console = console;
        }

        public CommandType CommandType => CommandType.MySql;

        public string[] CommandName => new[] { "i", "init" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Initializing database...");
            _console.WriteLine();

            using (var connection = new MySqlConnection(Database.GetConnectionString(settings)))
            {
                connection.Open();

                if (IsTableAvailable(connection))
                {
                    _console.WriteLine("Table 'infrastructure.dbscripts' already exists");
                    return 0;
                }

                var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MySql.Resources.DbScriptsTable.mysql");

                if (string.IsNullOrEmpty(script))
                {
                    return 1;
                }

                using (var command = new MySqlCommand(script, connection))
                {
                    _console.WriteLine("Creating table 'infrastructure.dbscripts'");
                    command.ExecuteNonQuery();
                }
            }

            return 0;
        }

        public static bool IsTableAvailable(MySqlConnection connection)
        {
            using (var command = new MySqlCommand("SHOW TABLES LIKE 'infrastructure.dbscripts'", connection))
            {
                var executeScalar = command.ExecuteScalar();
                return executeScalar != null;
            }
        }
    }
}


