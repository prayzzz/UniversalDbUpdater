using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql.Commands;
using UniversalDbUpdater.Test;

namespace UniversalDbUpdater.MsSql.Test.Commands
{
    [TestFixture]
    public class BackupCommandTest2
    {
        private static readonly Settings Settings = Setup.Settings;

        [Test]
        public void Test_Type()
        {
            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new BackupCommand(consoleMock.Object, dateTimeMock.Object);

            Assert.AreEqual(DatabaseType.MsSql, command.DatabaseType);
        }

        [Test]
        public void Test_Parameters()
        {
            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new BackupCommand(consoleMock.Object, dateTimeMock.Object);

            Assert.Contains("b", command.CommandName);
            Assert.Contains("backup", command.CommandName);
        }

        [Test]
        public void Test_Backup_Created_Correctly()
        {
            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new BackupCommand(consoleMock.Object, dateTimeMock.Object);

            var returnCode = command.Execute(new List<string>(), Settings);

            Assert.AreEqual(0, returnCode);
            Assert.True(Directory.Exists(Settings.BackupDirectory));

            var expectedFilePath = Path.Combine(Settings.BackupDirectory, string.Format("{0}-{1}.bak", now.ToString(Constants.DateFormat), Settings.Database));
            Assert.True(File.Exists(expectedFilePath));

            Directory.Delete(Settings.BackupDirectory, true);
            Assert.False(Directory.Exists(Settings.BackupDirectory));
        }
    }
}
