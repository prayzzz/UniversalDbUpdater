using System;
using System.Data.SqlClient;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql
{
    public class Database
    {
        public static string GetConnectionString(Settings settings)
        {
            return $"server={settings.Host};port={settings.Port};uid={settings.User};pwd={settings.Password};database={settings.Database};";
        }

        public static bool IsDbScriptsTableAvailable(Settings settings)
        {
            using (var connection = new SqlConnection(GetConnectionString(settings)))
            {
                //if (!InitCommand.IsTableAvailable(connection))
                //{
                //    Console.WriteLine("Table 'infrastructure.dbscripts' not available");
                //    Console.WriteLine("Use -i first");

                //    return false;
                //}
            }

            return true;
        }
    }
}