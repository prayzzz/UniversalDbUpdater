using System.Collections.Generic;
using System.Data.SqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;
using UniversalDbUpdater.Test;

namespace UniversalDbUpdater.MsSql.Test.Commands
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

            Assert.AreEqual(DatabaseType.MsSql, command.DatabaseType);
        }

        [Test]
        public void Test_Parameters()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);

            Assert.Contains("-i", command.Parameters);
            Assert.Contains("--init", command.Parameters);
        }

        [Test]
        public void Test_Execute_With_Existing_Scripts_Table()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.ConnectionString, Settings.Database);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.ConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new SqlCommand("SELECT * FROM information_schema.tables WHERE TABLE_SCHEMA = 'Infrastructure' AND TABLE_NAME = 'DbScripts'", connection))
                {
                    Assert.AreEqual("UniversalDbUpdaterTest", mysqlCommand.ExecuteScalar());
                }
            }

            MsSqlTestHelper.DropScriptsTable(Setup.ConnectionString, Settings.Database);
        }

        [Test]
        public void Test_Execute_Without_Existing_Scripts_Table()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.ConnectionString, Settings.Database);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.ConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new SqlCommand("SELECT * FROM information_schema.tables WHERE TABLE_SCHEMA = 'Infrastructure' AND TABLE_NAME = 'DbScripts'", connection))
                {
                    Assert.AreEqual("UniversalDbUpdaterTest", mysqlCommand.ExecuteScalar());
                }
            }

            MsSqlTestHelper.DropScriptsTable(Setup.ConnectionString, Settings.Database);
        }
    }
}