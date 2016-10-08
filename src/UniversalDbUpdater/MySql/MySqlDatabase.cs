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
            return $"server={settings.Host};port={settings.Port};uid={settings.User};pwd={settings.Password};database={settings.Database};";
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