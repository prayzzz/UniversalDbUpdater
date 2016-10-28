using System;
using System.Reflection;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql;

namespace UniversalDbUpdater.Test.MySql
{
    public static class MySqlTestHelper
    {
        public static void CreateScriptsTable(string connectionString, Settings settings)
        {
            var script = ResourceHelper.Current.GetEmbeddedFile(typeof(MySqlDatabase).GetTypeInfo().Assembly, "UniversalDbUpdater.MySql.Resources.DbScriptsTable.mysql");

            if (string.IsNullOrEmpty(script))
            {
                Console.WriteLine("## TestHelper: DbScriptsTable.mysql not available");
                Assert.Fail();
            }

            script = script.Replace("##SCRIPTTABLE##", MySqlDatabase.GetTableName(settings.Schema, settings.Table));

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(settings.Database);

                using (var command = new MySqlCommand(script, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: Scripts table created");
                }
            }
        }

        public static void DropScriptsTable(string connectionString, Settings settings)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                connection.ChangeDatabase(settings.Database);

                using (var command = new MySqlCommand($"DROP TABLE IF EXISTS `{MySqlDatabase.GetTableName(settings.Schema, settings.Table)}`", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("## TestHelper: Scripts table dropped");
                }
            }
        }
    }
}