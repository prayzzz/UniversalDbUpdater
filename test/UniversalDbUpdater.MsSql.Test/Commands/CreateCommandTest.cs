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
    public class CreateCommandTest
    {
        [Test]
        public void Test_Type()
        {
            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new CreateCommand(consoleMock.Object, dateTimeMock.Object);

            Assert.AreEqual(DatabaseType.MsSql, command.DatabaseType);
        }

        [Test]
        public void Test_Command()
        {
            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new CreateCommand(consoleMock.Object, dateTimeMock.Object);

            Assert.Contains("-c", command.Parameters);
            Assert.Contains("--create", command.Parameters);
        }

        [Test]
        public void Test_Script_Created_Correctly_With_Scriptname()
        {
            const string name = "Test Name";
            const string description = "Test Description";

            var now = DateTime.Now;

            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();
            consoleMock.Setup(x => x.ReadLine()).Returns(description);

            var dateTimeMock = TestHelper.CreateDateTimeMock(now);

            var command = new CreateCommand(consoleMock.Object, dateTimeMock.Object);

            var returnCode = command.Execute(new List<string> { name }, Setup.Settings);

            Assert.AreEqual(0, returnCode);

            var expectedFileName = now.ToString(Constants.DateFormat) + "_" + name.Replace(" ", "_") + ".sql";
            Assert.True(File.Exists(expectedFileName));

            var fileContent = File.ReadAllText(expectedFileName);
            Assert.True(fileContent.Contains(name));
            Assert.True(fileContent.Contains(description));
            Assert.True(fileContent.Contains(now.ToString("s")));

            File.Delete(expectedFileName);
            Assert.False(File.Exists(expectedFileName));
        }
    }
}