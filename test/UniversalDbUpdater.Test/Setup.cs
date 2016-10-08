using System;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.Test
{
    [SetUpFixture]
    public class Setup
    {
        public const string MsSqlConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true";
        public const string MySqlConnectionString = "server=127.0.0.1;port=3306;uid=root;pwd=admin;";
        private const string DbName = "UniversalDbUpdaterTest";

        public static readonly Settings MySqlSettings;
        public static readonly Settings MsSqlSettings;

        static Setup()
        {
            MySqlSettings = new Settings
            {
                Host = "127.0.0.1",
                Port = 3306,
                User = "root",
                Password = "admin",
                Database = DbName,
                BackupDirectory = "./backup"
            };

            MsSqlSettings = new Settings
            {
                Host = @"(localdb)\MSSQLLocalDB",
                Database = DbName,
                BackupDirectory = "./backup",
                IntegratedSecurity = true
            };
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Console.WriteLine(" ");
            Console.WriteLine("## Setup start");

            using (var connection = new MySqlConnection(MySqlConnectionString))
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

            using (var connection = new SqlConnection(MsSqlConnectionString))
            {
                connection.Open();

                if (IsDatabaseAvailable(connection, DbName))
                {
                    using (var command = new SqlCommand($"DROP DATABASE {DbName}", connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Database dropped");
                    }
                }

                using (var command = new SqlCommand($"CREATE DATABASE {DbName}", connection))
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

            using (var connection = new MySqlConnection(MySqlConnectionString))
            using (var command = new MySqlCommand($"DROP DATABASE {DbName}", connection))
            {
                connection.Open();

                command.ExecuteNonQuery();
                Console.WriteLine("Database dropped");
            }

            using (var connection = new SqlConnection(MsSqlConnectionString))
            using (var command = new SqlCommand($"ALTER DATABASE {DbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE \n DROP DATABASE {DbName}", connection))
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
        private static bool IsDatabaseAvailable(SqlConnection connection, string name)
        {
            using (var command = new SqlCommand($"SELECT * FROM master.dbo.sysdatabases WHERE [name]='{name}'", connection))
            {
                return command.ExecuteScalar() != null;
            }
        }
    }
}