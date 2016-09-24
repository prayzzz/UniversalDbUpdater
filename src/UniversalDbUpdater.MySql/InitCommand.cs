using System;
using System.Collections.Generic;
using System.Reflection;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql
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
            using (var connection = new MySqlConnection(settings.DbConnection))
            {
                connection.Open();

                if (IsSchemaAvailable(connection))
                {
                    Console.WriteLine("Table 'infrastructure.dbscripts' already exists.");
                    return 0;
                }

                var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MySql.Resources.DbScriptsTable.mysql");

                if (!string.IsNullOrEmpty(script))
                {
                    return 1;
                }


            }

            return 0;
        }

        private bool IsSchemaAvailable(MySqlConnection connection)
        {
            using (var command = new MySqlCommand("SHOW TABLES LIKE 'infrastructure.dbscripts'", connection))
            using (var reader = command.ExecuteReader())
            {

                return reader.HasRows;
            }
        }

        public void HelpShort()
        {
            Console.WriteLine("\t -i \t First time initialization.");
        }
    }
}


