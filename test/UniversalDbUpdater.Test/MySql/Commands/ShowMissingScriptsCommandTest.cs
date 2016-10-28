using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UniversalDbUpdater.Common;
using UniversalDbUpdater.MySql.Commands;

namespace UniversalDbUpdater.Test.MySql.Commands
{
    [TestFixture]
    public class ShowMissingScriptsCommandTest
    {
        [TearDown]
        public void TearDown()
        {
            File.Delete(Script01);
            Assert.False(File.Exists(Script01));

            File.Delete(Script02);
            Assert.False(File.Exists(Script02));
            
            MySqlTestHelper.DropScriptsTable(Setup.MySqlConnectionString, Settings);
        }

        private const string Script01 = "2016-10-01_18-00-00_Script01.mysql";
        private const string Script02 = "2016-10-01_19-00-00_Script02.mysql";

        private static readonly Settings Settings = Setup.MySqlSettings;

        [Test]
        public void Test_GetMissingScripts()
        {
            MySqlTestHelper.CreateScriptsTable(Setup.MySqlConnectionString, Settings);

            // copy scripts from resources
            var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.Test.MySql.Resources.{Script01}");
            File.WriteAllText(Script01, script);

            script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, $"UniversalDbUpdater.Test.MySql.Resources.{Script02}");
            File.WriteAllText(Script02, script);

            var missingScripts = ShowMissingScriptsCommand.GetMissingScripts(Settings);

            Assert.AreEqual(2, missingScripts.Count());
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
        public void Test_Type()
        {
            var consoleMock = TestHelper.CreateConsoleMock().SetupWriteLineToConsole();

            var command = new ShowMissingScriptsCommand(consoleMock.Object);

            Assert.AreEqual(CommandType.MySql, command.CommandType);
        }
    }
}