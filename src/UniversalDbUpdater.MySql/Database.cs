using System;
using MySql.Data.MySqlClient;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql.Commands;

namespace UniversalDbUpdater.MySql
{
    public class Database
    {
        public static string ConnectionString(Settings settings)
        {
            return $"server={settings.Host};port={settings.Port};uid={settings.User};pwd={settings.Password};database={settings.Database};";
        }

        public static bool IsDbScriptsTableAvailable(Settings settings)
        {
            using (var connection = new MySqlConnection(ConnectionString(settings)))
            {
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