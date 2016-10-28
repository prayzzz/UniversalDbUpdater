using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MsSql;
using UniversalDbUpdater.MsSql.Commands;

namespace UniversalDbUpdater.Test.MsSql.Commands
{
    [TestFixture]
    public class CreateCommandTest
    {
        [TearDown]
        public void TearDown()
        {
            var files = Directory.GetFiles(".", "*.sql");

            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        private static readonly Settings Settings = Setup.MsSqlSettings;

        [Test]
        public void Test_Command()
        {
            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new CreateCommand(consoleMock.Object, dateTimeMock.Object);

            Assert.Contains("c", command.CommandName);
            Assert.Contains("create", command.CommandName);
        }

        [Test]
        public void Test_Script_Created_Correctly_With_Scriptname()
        {
            const string name = "Test Name";

            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            consoleMock.Setup(x => x.ReadLine()).Returns(name);

            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new CreateCommand(consoleMock.Object, dateTimeMock.Object);

            var returnCode = command.Execute(new List<string> { name }, Setup.MsSqlSettings);

            Assert.AreEqual(0, returnCode);

            var fileName = now.ToString(Constants.DateFormat) + "_" + name.Replace(" ", "_");
            var expectedFile = fileName + ".sql";
            Assert.True(File.Exists(expectedFile));

            var fileContent = File.ReadAllText(expectedFile);
            Assert.True(fileContent.Contains(MsSqlDatabase.GetTableName(Settings.Schema, Settings.Table)));
            Assert.True(fileContent.Contains(fileName));

            File.Delete(expectedFile);
            Assert.False(File.Exists(expectedFile));
        }

        [Test]
        public void Test_Type()
        {
            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new CreateCommand(consoleMock.Object, dateTimeMock.Object);

            Assert.AreEqual(CommandType.MsSql, command.CommandType);
        }
    }
}