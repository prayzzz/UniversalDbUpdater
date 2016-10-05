using System;
using System.Data.SqlClient;
using System.Text;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;

namespace UniversalDbUpdater.MsSql
{
    public class Database
    {
        public static string GetConnectionString(Settings settings)
        {
            var builder = new StringBuilder();
            builder.Append($"Server={settings.Host};");
            builder.Append($"Database={settings.Database};");

            if (settings.IntegratedSecurity)
            {
                builder.Append($"Integrated Security={settings.IntegratedSecurity};");
            }
            else
            {
                builder.Append($"User Id={settings.User};");
                builder.Append($"Password={settings.Password};");
            }

            return builder.ToString();
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