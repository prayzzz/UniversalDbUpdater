using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql;
using UniversalDbUpdater.MsSql.Commands;

namespace UniversalDbUpdater.Test.MsSql.Commands
{
    [TestFixture]
    public class ExecuteMissingScriptsCommandTest
    {
        private const string Script01 = "2016-10-01_18-00-00_Script01.sql";
        private const string Script02 = "2016-10-01_19-00-00_Script02.sql";

        private static readonly Settings Settings = Setup.MsSqlSettings;

        [Test]
        public void Test_Type()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);

            Assert.AreEqual(CommandType.MsSql, command.CommandType);
        }

        [Test]
        public void Test_Parameters()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);

            Assert.Contains("e", command.CommandName);
            Assert.Contains("execute", command.CommandName);
        }

        [Test]
        public void Test_Execute_With_Missing_Scripts()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.MsSqlConnectionString, Settings);

            // copy scripts from resources
            var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.Test.MsSql.Resources.{Script01}");
            File.WriteAllText(Script01, script);

            script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.Test.MsSql.Resources.{Script02}");
            File.WriteAllText(Script02, script);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.MsSqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var sqlCommand = new SqlCommand($"SELECT * FROM {MsSqlDatabase.GetTableName(Settings.Schema, Settings.Table)}", connection))
                using (var reader = sqlCommand.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual("2016-10-01_18-00-00_Script01", reader.GetString(1));

                    reader.Read();
                    Assert.AreEqual("2016-10-01_19-00-00_Script02", reader.GetString(1));
                }
            }
        }
        
        [Test]
        public void Test_Execute_Without_Missing_Scripts()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.MsSqlConnectionString, Settings);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.MsSqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var sqlCommand = new SqlCommand($"SELECT * FROM {MsSqlDatabase.GetTableName(Settings.Schema, Settings.Table)}", connection))
                using (var reader = sqlCommand.ExecuteReader())
                {
                    Assert.False(reader.HasRows);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(Script01);
            Assert.False(File.Exists(Script01));

            File.Delete(Script02);
            Assert.False(File.Exists(Script02));
            
            MsSqlTestHelper.DropScriptsTable(Setup.MsSqlConnectionString, Settings);
        }
    }
}