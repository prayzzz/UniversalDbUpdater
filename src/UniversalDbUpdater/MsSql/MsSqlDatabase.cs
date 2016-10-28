using System;
using System.Data.SqlClient;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;

namespace UniversalDbUpdater.MsSql
{
    public class MsSqlDatabase
    {
        public static string GetConnectionString(Settings settings)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = settings.Host,
                InitialCatalog = settings.Database,
            };

            if (settings.IntegratedSecurity)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = settings.User;
                builder.Password = settings.Password;
            }

            return builder.ToString();
        }

        public static bool IsDbScriptsTableAvailable(Settings settings)
        {
            using (var connection = new SqlConnection(GetConnectionString(settings)))
            {
                connection.Open();

                if (!InitCommand.IsTableAvailable(connection, settings))
                {
                    Console.WriteLine($"Table '{GetTableName(settings.Schema, settings.Table)}' not available");
                    Console.WriteLine("Use -i first");

                    return false;
                }
            }

            return true;
        }

        public static string GetTableName(string schema, string table)
        {
            if (string.IsNullOrEmpty(schema))
            {
                return table;
            }

            return $"{schema}.{table}";
        }
    }
}