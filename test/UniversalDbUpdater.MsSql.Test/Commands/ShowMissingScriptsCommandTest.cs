﻿using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;
using UniversalDbUpdater.Test;

namespace UniversalDbUpdater.MsSql.Test.Commands
{
    [TestFixture]
    public class ShowMissingScriptsCommandTest
    {
        private const string Script01 = "2016-10-01_18-00-00_Script01.sql";
        private const string Script02 = "2016-10-01_19-00-00_Script02.sql";

        private static readonly Settings Settings = Setup.Settings;

        [Test]
        public void Test_Type()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ShowMissingScriptsCommand(consoleMock.Object);

            Assert.AreEqual(DatabaseType.MsSql, command.DatabaseType);
        }

        [Test]
        public void Test_Parameters()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ShowMissingScriptsCommand(consoleMock.Object);

            Assert.Contains("s", command.CommandName);
            Assert.Contains("show", command.CommandName);
        }

        [Test]
        public void Test_GetMissingScripts()
        {
            MsSqlTestHelper.CreateScriptsTable(Setup.ConnectionString, Settings.Database);

            // copy scripts from resources
            var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.MsSql.Test.Resources.{Script01}");
            File.WriteAllText(Script01, script);

            script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.MsSql.Test.Resources.{Script02}");
            File.WriteAllText(Script02, script);

            var missingScripts = ShowMissingScriptsCommand.GetMissingScripts(Settings);

            Assert.AreEqual(2, missingScripts.Count());

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