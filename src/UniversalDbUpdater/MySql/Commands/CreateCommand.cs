using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UniversalDbUpdater.Common;
using static UniversalDbUpdater.MySql.MySqlDatabase;

namespace UniversalDbUpdater.MySql.Commands
{
    public class CreateCommand : ICommand
    {
        private readonly IConsoleFacade _console;
        private readonly IDateTimeFacade _dateTime;

        public CreateCommand(IConsoleFacade console, IDateTimeFacade dateTime)
        {
            _console = console;
            _dateTime = dateTime;
        }

        public CommandType CommandType => CommandType.MySql;

        public string[] CommandName => new[] { "c", "create" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Creating new script...");
            _console.WriteLine();

            var scriptName = string.Empty;
            while (string.IsNullOrEmpty(scriptName))
            {
                _console.WriteLine("Script name:");
                scriptName = _console.ReadLine();
                _console.WriteLine();
            }

            var scriptFileName = _dateTime.Now.ToString(Constants.DateFormat) + "_" + scriptName.Replace(" ", "_");

            var file = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MySql.Resources.ScriptTemplate.mysql");
            file = file.Replace("##SCRIPTTABLE##", GetTableName(settings.Schema, settings.Table));
            file = file.Replace("##FILENAME##", scriptFileName);

            var scriptFile = Path.Combine(settings.ScriptsDirectory, scriptFileName + ".mysql");
            File.WriteAllText(scriptFile, file);

            _console.WriteLine();
            _console.WriteLine($"Script created: {scriptFile}");

            return 0;
        }
    }
}