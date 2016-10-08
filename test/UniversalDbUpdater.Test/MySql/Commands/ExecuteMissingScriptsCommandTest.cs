﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql.Commands;

namespace UniversalDbUpdater.Test.MySql.Commands
{
    [TestFixture]
    public class ExecuteMissingScriptsCommandTest
    {
        private const string Script01 = "2016-10-01_18-00-00_Script01.mysql";
        private const string Script02 = "2016-10-01_19-00-00_Script02.mysql";

        private static readonly Settings Settings = Setup.MySqlSettings;

        [Test]
        public void Test_Type()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);

            Assert.AreEqual(CommandType.MySql, command.CommandType);
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
            MySqlTestHelper.CreateScriptsTable(Setup.MySqlConnectionString, Settings.Database);

            // copy scripts from resources
            var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.Test.MySql.Resources.{Script01}");
            File.WriteAllText(Script01, script);

            script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.Test.MySql.Resources.{Script02}");
            File.WriteAllText(Script02, script);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new MySqlConnection(Setup.MySqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new MySqlCommand("SELECT * FROM `infrastructure.dbscripts`", connection))
                using (var reader = mysqlCommand.ExecuteReader())
                {
                    reader.Read();
                    Assert.AreEqual(new DateTime(2016, 10, 01, 18, 00, 00), reader.GetDateTime("Date"));
                    Assert.AreEqual("Script01", reader.GetString("Name"));
                    Assert.AreEqual("Script01Description", reader.GetString("Description"));

                    reader.Read();
                    Assert.AreEqual(new DateTime(2016, 10, 01, 19, 00, 00), reader.GetDateTime("Date"));
                    Assert.AreEqual("Script02", reader.GetString("Name"));
                    Assert.AreEqual("Script02Description", reader.GetString("Description"));
                }
            }

            MySqlTestHelper.DropScriptsTable(Setup.MySqlConnectionString, Settings.Database);
        }

        [Test]
        public void Test_Execute_Without_Missing_Scripts()
        {
            MySqlTestHelper.CreateScriptsTable(Setup.MySqlConnectionString, Settings.Database);

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ExecuteMissingScriptsCommand(consoleMock.Object);
            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);

            using (var connection = new MySqlConnection(Setup.MySqlConnectionString))
            {
                connection.Open();
                connection.ChangeDatabase(Settings.Database);

                using (var mysqlCommand = new MySqlCommand("SELECT * FROM `infrastructure.dbscripts`", connection))
                using (var reader = mysqlCommand.ExecuteReader())
                {
                    Assert.False(reader.HasRows);
                }
            }

            MySqlTestHelper.DropScriptsTable(Setup.MySqlConnectionString, Settings.Database);
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