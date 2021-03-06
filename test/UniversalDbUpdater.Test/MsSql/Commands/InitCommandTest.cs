using System.Collections.Generic;
using System.Data.SqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;

namespace UniversalDbUpdater.Test.MsSql.Commands
{
    [TestFixture]
    public class InitCommandTest
    {
        [TearDown]
        public void TearDown()
        {
            MsSqlTestHelper.DropScriptsTable(Setup.MsSqlConnectionString, Settings);
        }

        private static readonly Settings Settings = Setup.MsSqlSettings;

        [Test]
        public void Test_Execute_With_Existing_Scripts_Table()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.MsSqlConnectionString, Settings);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.MsSqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);
                
                var schema = Settings.Schema.Replace("[", "").Replace("]", "");
                var table = Settings.Table.Replace("[", "").Replace("]", "");

                using (var mysqlCommand = new SqlCommand($"SELECT * FROM information_schema.tables WHERE TABLE_SCHEMA = '{schema}' AND TABLE_NAME = '{table}'", connection))
                {
                    Assert.AreEqual("UniversalDbUpdaterTest", mysqlCommand.ExecuteScalar());
                }
            }
        }

        [Test]
        public void Test_Execute_Without_Existing_Scripts_Table()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new InitCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.MsSqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);
                
                var schema = Settings.Schema.Replace("[", "").Replace("]", "");
                var table = Settings.Table.Replace("[", "").Replace("]", "");

                using (var mysqlCommand = new SqlCommand($"SELECT * FROM information_schema.tables WHERE TABLE_SCHEMA = '{schema}' AND TABLE_NAME = '{table}'", connection))
                {
                    Assert.AreEqual("UniversalDbUpdaterTest", mysqlCommand.ExecuteScalar());
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

            Assert.AreEqual(CommandType.MsSql, command.CommandType);
        }
    }
}