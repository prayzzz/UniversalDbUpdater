﻿using System;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Test
{
    [SetUpFixture]
    public class Setup
    {
        //database={settings.Database};
        public const string ConnectionString = "server=127.0.0.1;port=3306;uid=root;pwd=admin;";
        private const string DbName = "UniversalDbUpdaterTest";

        public static readonly Settings Settings;

        static Setup()
        {
            Settings = new Settings
            {
                Host = "127.0.0.1",
                Port = 3306,
                User = "root",
                Password = "admin",
                Database = DbName,
                BackupDirectory = "./backup"
            };
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Console.WriteLine(" ");
            Console.WriteLine("## Setup start");

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                if (IsDatabaseAvailable(connection, DbName))
                {
                    using (var command = new MySqlCommand($"DROP DATABASE {DbName}", connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Database dropped");
                    }
                }

                using (var command = new MySqlCommand($"CREATE DATABASE {DbName}", connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Database created");
                }
            }

            Console.WriteLine("## Setup end");
            Console.WriteLine(" ");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Console.WriteLine(" ");
            Console.WriteLine("## Teardown start");

            using (var connection = new MySqlConnection(ConnectionString))
            using (var command = new MySqlCommand($"DROP DATABASE {DbName}", connection))
            {
                connection.Open();

                command.ExecuteNonQuery();
                Console.WriteLine("Database dropped");
            }

            Console.WriteLine("## Teardown end");
            Console.WriteLine(" ");
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