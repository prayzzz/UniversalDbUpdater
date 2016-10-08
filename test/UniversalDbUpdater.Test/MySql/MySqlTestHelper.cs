using System;
using System.Reflection;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.Test.MySql
{
    public static class MySqlTestHelper
    {
        public static void CreateScriptsTable(string connectionString, string database)
        {
            var script = ResourceHelper.Current.GetEmbeddedFile(typeof(MySqlTestHelper).GetTypeInfo().Assembly, "UniversalDbUpdater.Test.MySql.Resources.DbScriptsTable.mysql");

            if (string.IsNullOrEmpty(script))
            {
                Console.WriteLine("## TestHelper: DbScriptsTable.mysql not available");
                Assert.Fail();
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(database);

                using (var command = new MySqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: DbScripts table created");
                }
            }
        }

        public static void DropScriptsTable(string connectionString, string database)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(database);

                using (var command = new MySqlCommand("DROP TABLE `infrastructure.dbscripts`", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: DbScripts table dropped");
                }
            }
        }
    }
}