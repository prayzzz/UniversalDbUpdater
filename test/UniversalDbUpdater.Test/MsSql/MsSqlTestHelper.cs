using System;
using System.Data.SqlClient;
using System.Reflection;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql;

namespace UniversalDbUpdater.Test.MsSql
{
    public static class MsSqlTestHelper
    {
        public static void CreateScriptsTable(string connectionString, Settings settings)
        {
            var script = ResourceHelper.Current.GetEmbeddedFile(typeof(MsSqlDatabase).GetTypeInfo().Assembly, "UniversalDbUpdater.MsSql.Resources.DbScriptsTable.sql");

            if (string.IsNullOrEmpty(script))
            {
                Console.WriteLine("## TestHelper: DbScriptsTable.sql not available");
                Assert.Fail();
            }

            script = script.Replace("##SCRIPTTABLE##", MsSqlDatabase.GetTableName(settings.Schema, settings.Table));

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(settings.Database);

                using (var command = new SqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: Scripts table created");
                }
            }
        }

        public static void DropScriptsTable(string connectionString, Settings settings)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(settings.Database);

                using (var command = new SqlCommand($"DROP TABLE IF EXISTS {MsSqlDatabase.GetTableName(settings.Schema, settings.Table)} \r\n DROP SCHEMA IF EXISTS {settings.Schema}", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: Scripts table dropped");
                }
            }
        }
    }
}