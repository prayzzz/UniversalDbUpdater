using System;
using System.Collections.Generic;
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


