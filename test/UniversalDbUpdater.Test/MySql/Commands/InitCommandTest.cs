using System.Collections.Generic;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql;
using UniversalDbUpdater.MySql.Commands;

namespace UniversalDbUpdater.Test.MySql.Commands
{
    [TestFixture]
    public class InitCommandTest
    {
        [TearDown]
        public void TearDown()
        {
            MySqlTestHelper.DropScriptsTable(Setup.MySqlConnectionString, Settings);
        }

        private static readonly Settings Settings = Setup.MySqlSettings;

        [Test]
        public void Test_Execute_With_Existing_Scripts_Table()
        {
            MySqlTestHelper.CreateScriptsTable(Setup.MySqlConnectionString, Settings);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new MySqlConnection(Setup.MySqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new MySqlCommand($"SHOW TABLES LIKE '{MySqlDatabase.GetTableName(Settings.Schema, Settings.Table)}'", connection))
                {
                    Assert.AreEqual(MySqlDatabase.GetTableName(Settings.Schema, Settings.Table), mysqlCommand.ExecuteScalar());
                }
            }
        }

        [Test]
        public void Test_Execute_Without_Existing_Scripts_Table()
        {
            MySqlTestHelper.CreateScriptsTable(Setup.MySqlConnectionString, Settings);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new MySqlConnection(Setup.MySqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new MySqlCommand($"SHOW TABLES LIKE '{MySqlDatabase.GetTableName(Settings.Schema, Settings.Table)}'", connection))
                {
                    Assert.AreEqual(MySqlDatabase.GetTableName(Settings.Schema, Settings.Table), mysqlCommand.ExecuteScalar());
                }
            }
        }

        [Test]
        public void Test_Parameters()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);

            Assert.Contains("i", command.CommandName);
            Assert.Contains("init", command.CommandName);
        }

        [Test]
        public void Test_Type()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);

            Assert.AreEqual(CommandType.MySql, command.CommandType);
        }
    }
}