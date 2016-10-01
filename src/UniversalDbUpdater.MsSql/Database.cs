using System;
using System.Data.SqlClient;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;

namespace UniversalDbUpdater.MsSql
{
    public class Database
    {
        public static string GetConnectionString(Settings settings)
        {
            return $"Server={settings.Host};Integrated Security={settings.IntegratedSecurity};Database={settings.Database}";
        }

        public static bool IsDbScriptsTableAvailable(Settings settings)
        {
            using (var connection = new SqlConnection(GetConnectionString(settings)))
            {
                connection.Open();

                if (!InitCommand.IsTableAvailable(connection))
                {
                    Console.WriteLine("Table [Infrastructure].[DbScripts] not available");
                    Console.WriteLine("Use -i first");

                    return false;
                }
            }

            return true;
        }
    }
}