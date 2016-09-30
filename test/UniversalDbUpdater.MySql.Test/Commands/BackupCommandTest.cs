using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql.Commands;

namespace UniversalDbUpdater.MySql.Test.Commands
{
    [TestFixture]
    public class BackupCommandTest
    {
        [Test]
        public void Test_Type()
        {
            Assert.AreEqual(DatabaseType.MySql, BackupCommand.Current.DatabaseType);
        }

        [Test]
        public void Test_Backup()
        {
            var command = BackupCommand.Current;

            command.Execute(new List<string>(), Setup.Settings);

            Assert.True(Directory.Exists(Setup.Settings.BackupDirectory));
            Assert.AreEqual(Directory.GetFiles(Setup.Settings.BackupDirectory).Length, 1);
        }
    }
}
