using System.Collections.Generic;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql.Commands;
using UniversalDbUpdater.Test;

namespace UniversalDbUpdater.MySql.Test.Commands
{
    [TestFixture]
    public class InitCommandTest
    {
        private static readonly Settings Settings = Setup.Settings;

        [Test]
        public void Test_Type()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);

            Assert.AreEqual(DatabaseType.MySql, command.DatabaseType);
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
        public void Test_Execute_With_Existing_Scripts_Table()
        {
            MySqlTestHelper.CreateScriptsTable(Setup.ConnectionString, Settings.Database);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new MySqlConnection(Setup.ConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new MySqlCommand("SHOW TABLES LIKE 'infrastructure.dbscripts'", connection))
                {
                    Assert.AreEqual("infrastructure.dbscripts", mysqlCommand.ExecuteScalar());
                }
            }

            MySqlTestHelper.DropScriptsTable(Setup.ConnectionString, Settings.Database);
        }

        [Test]
        public void Test_Execute_Without_Existing_Scripts_Table()
        {
            MySqlTestHelper.CreateScriptsTable(Setup.ConnectionString, Settings.Database);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new MySqlConnection(Setup.ConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new MySqlCommand("SHOW TABLES LIKE 'infrastructure.dbscripts'", connection))
                {
                    Assert.AreEqual("infrastructure.dbscripts", mysqlCommand.ExecuteScalar());
                }
            }

            MySqlTestHelper.DropScriptsTable(Setup.ConnectionString, Settings.Database);
        }
    }
}