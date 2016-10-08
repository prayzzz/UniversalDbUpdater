using System;
using System.Data.SqlClient;
using System.Reflection;
using NUnit.Framework;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Test
{
    public static class MsSqlTestHelper
    {
        public static void CreateScriptsTable(string connectionString, string database)
        {
            var script = ResourceHelper.Current.GetEmbeddedFile(typeof(MsSqlTestHelper).GetTypeInfo().Assembly, "UniversalDbUpdater.MsSql.Test.Resources.DbScriptsTable.sql");

            if (string.IsNullOrEmpty(script))
            {
                Console.WriteLine("## TestHelper: DbScriptsTable.sql not available");
                Assert.Fail();
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(database);

                using (var command = new SqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: DbScripts table created");
                }
            }
        }

        public static void DropScriptsTable(string connectionString, string database)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(database);

                using (var command = new SqlCommand("DROP TABLE [Infrastructure].[DbScripts] \r\n DROP SCHEMA [Infrastructure]", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: DbScripts table dropped");
                }
            }
        }
    }
}