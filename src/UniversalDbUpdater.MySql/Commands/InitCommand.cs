using System;
using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Commands
{
    public class InitCommand : ICommand
    {
        private static InitCommand _instance;

        private InitCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new InitCommand());

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] Command => new[] { "-i", "--init" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            Console.WriteLine("Initializing database...");
            Console.WriteLine();

            using (var connection = new MySqlConnection(Database.GetConnectionString(settings)))
            {
                connection.Open();

                if (IsTableAvailable(connection))
                {
                    Console.WriteLine("Table 'infrastructure.dbscripts' already exists");
                    return 0;
                }

                var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MySql.Resources.DbScriptsTable.mysql");

                if (string.IsNullOrEmpty(script))
                {
                    return 1;
                }

                using (var command = new MySqlCommand(script, connection))
                {
                    Console.WriteLine("Creating table 'infrastructure.dbscripts'");
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static bool IsTableAvailable(MySqlConnection connection)
        {
            using (var command = new MySqlCommand("SHOW TABLES LIKE 'infrastructure.dbscripts'", connection))
            {
                var executeScalar = command.ExecuteScalar();
                return executeScalar != null;
            }
        }

        public void HelpShort()
        {
            Console.WriteLine(" -i --init \t First time initialization.");
        }
    }
}


