using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;
using UniversalDbUpdater.Test;

namespace UniversalDbUpdater.MsSql.Test.Commands
{
    [TestFixture]
    public class ExecuteMissingScriptsCommandTest
    {
        private const string Script01 = "2016-10-01_18-00-00_Script01.sql";
        private const string Script02 = "2016-10-01_19-00-00_Script02.sql";

        private static readonly Settings Settings = Setup.Settings;

        [Test]
        public void Test_Type()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);

            Assert.AreEqual(DatabaseType.MsSql, command.DatabaseType);
        }

        [Test]
        public void Test_Parameters()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);

            Assert.Contains("-e", command.Parameters);
            Assert.Contains("--execute", command.Parameters);
        }

        [Test]
        public void Test_Execute_With_Missing_Scripts()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.ConnectionString, Settings.Database);

            // copy scripts from resources
            var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.MsSql.Test.Resources.{Script01}");
            File.WriteAllText(Script01, script);

            script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.MsSql.Test.Resources.{Script02}");
            File.WriteAllText(Script02, script);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.ConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var sqlCommand = new SqlCommand("SELECT * FROM [Infrastructure].[DbScripts]", connection))
                using (var reader = sqlCommand.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(new DateTime(2016, 10, 01, 18, 00, 00), reader.GetDateTime(1));
                    Assert.AreEqual("Script01", reader.GetString(2));
                    Assert.AreEqual("Script01Description", reader.GetString(3));

                    reader.Read();
                    Assert.AreEqual(new DateTime(2016, 10, 01, 19, 00, 00), reader.GetDateTime(1));
                    Assert.AreEqual("Script02", reader.GetString(2));
                    Assert.AreEqual("Script02Description", reader.GetString(3));
                }
            }

            MsSqlTestHelper.DropScriptsTable(Setup.ConnectionString, Settings.Database);
        }
        
        [Test]
        public void Test_Execute_Without_Missing_Scripts()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.ConnectionString, Settings.Database);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new SqlConnection(Setup.ConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var sqlCommand = new SqlCommand("SELECT * FROM [Infrastructure].[DbScripts]", connection))
                using (var reader = sqlCommand.ExecuteReader())
                {
                    Assert.False(reader.HasRows);
                }
            }

            MsSqlTestHelper.DropScriptsTable(Setup.ConnectionString, Settings.Database);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(Script01);
            Assert.False(File.Exists(Script01));

            File.Delete(Script02);
            Assert.False(File.Exists(Script02));
        }
    }
}