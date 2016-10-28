using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;
using static UniversalDbUpdater.MySql.MySqlDatabase;

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

            using (var connection = new MySqlConnection(GetConnectionString(settings)))
            {
                connection.Open();

                if (IsTableAvailable(connection, settings))
                {
                    _console.WriteLine($"Table '{GetTableName(settings.Schema, settings.Table)}' already exists");
                    return 0;
                }

                var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MySql.Resources.DbScriptsTable.mysql");
                script = script.Replace("##SCRIPTTABLE##", GetTableName(settings.Schema, settings.Table));

                if (string.IsNullOrEmpty(script))
                {
                    return 1;
                }

                using (var command = new MySqlCommand(script, connection))
                {
                    _console.WriteLine($"Creating table '{GetTableName(settings.Schema, settings.Table)}'");
                    command.ExecuteNonQuery();
                }
            }

            return 0;
        }

        public static bool IsTableAvailable(MySqlConnection connection, Settings settings)
        {
            using (var command = new MySqlCommand($"SHOW TABLES LIKE '{GetTableName(settings.Schema, settings.Table)}'", connection))
            {
                var executeScalar = command.ExecuteScalar();
                return executeScalar != null;
            }
        }
    }
}


