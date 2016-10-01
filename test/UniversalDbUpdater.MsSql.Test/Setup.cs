using System;
using System.Data.SqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Test
{
    [SetUpFixture]
    public class Setup
    {
        public const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true";
        private const string DbName = "UniversalDbUpdaterTest";

        public static readonly Settings Settings;

        static Setup()
        {
            Settings = new Settings
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

            using (var connection = new SqlConnection(ConnectionString))
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

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand($"ALTER DATABASE {DbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE \n DROP DATABASE {DbName}", connection))
            {
                connection.Open();
                command.ExecuteNonQuery();

                Console.WriteLine("Database dropped");
            }

            Console.WriteLine("## Teardown end");
            Console.WriteLine(" ");
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