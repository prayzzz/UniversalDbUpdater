using System;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql.Commands;

namespace UniversalDbUpdater.MySql
{
    public class MySqlDatabase
    {
        public static string GetConnectionString(Settings settings)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Database = settings.Database,
                Server = settings.Host,
                Port = (uint) settings.Port,
                UserID = settings.User,
                Password = settings.Password
            };

            return builder.ToString();
        }

        public static bool IsDbScriptsTableAvailable(Settings settings)
        {
            using (var connection = new MySqlConnection(GetConnectionString(settings)))
            {
                connection.Open();

                if (!InitCommand.IsTableAvailable(connection))
                {
                    Console.WriteLine("Table 'infrastructure.dbscripts' not available");
                    Console.WriteLine("Use -i first");

                    return false;
                }
            }

            return true;
        }
    }
}