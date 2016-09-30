using System;
using System.IO;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Test
{
    [SetUpFixture]
    public class Setup
    {
        //database={settings.Database};
        private const string ConnectionString = "server=127.0.0.1;port=3306;uid=root;pwd=admin;";
        private const string DbName = "UniversalDbUpdaterTest";

        public static readonly Settings Settings;

        static Setup()
        {
            Settings = new Settings
            {
                Host = "127.0.0.1",
                Port = 3306,
                User = "admin",
                Password = "root",
                Database = DbName,
                BackupDirectory = "./backup"
            };
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                if (IsDatabaseAvailable(connection, DbName))
                {
                    using (var command = new MySqlCommand($"DROP DATABASE {DbName}", connection))
                    {
                        var executeScalar = command.ExecuteNonQuery();

                        if (executeScalar < 1)
                        {
                            Console.WriteLine($"Unexpected error: {executeScalar}");
                            return;
                        }

                        Console.WriteLine("Dropped existing Database");
                    }
                }

                using (var command = new MySqlCommand($"CREATE DATABASE {DbName}", connection))
                {
                    var executeScalar = command.ExecuteNonQuery();

                    if (executeScalar < 1)
                    {
                        Console.WriteLine($"Unexpected error: {executeScalar}");
                        return;
                    }

                    Console.WriteLine("Created Database");
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(Settings.BackupDirectory, true);

            using (var connection = new MySqlConnection(ConnectionString))
            using (var command = new MySqlCommand($"DROP DATABASE {DbName}", connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static bool IsDatabaseAvailable(MySqlConnection connection, string name)
        {
            using (var command = new MySqlCommand($"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{name}'", connection))
            {
                return command.ExecuteScalar() != null;
            }
        }
    }
}