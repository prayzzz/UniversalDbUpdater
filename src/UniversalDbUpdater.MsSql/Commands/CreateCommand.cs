using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Commands
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

        public CommandType CommandType => CommandType.MsSql;

        public string[] CommandName => new[] { "c", "create" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Creating new script...");
            _console.WriteLine();

            var script = new DbScript();
            script.Name = arguments.FirstOrDefault();

            while (string.IsNullOrEmpty(script.Name))
            {
                _console.WriteLine("Script name:");
                script.Name = _console.ReadLine();
                _console.WriteLine();
            }

            _console.WriteLine("Description:");
            script.Description = _console.ReadLine();
            script.Date = _dateTime.Now;

            var file = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MsSql.Resources.ScriptTemplate.sql");
            file = file.Replace("##DATE##", script.Date.ToString("s"));
            file = file.Replace("##NAME##", script.Name);
            file = file.Replace("##DESCRIPTION##", script.Description);

            File.WriteAllText(script.FileNameWithoutExtension + ".sql", file);

            _console.WriteLine($"Created script {script.FileNameWithoutExtension}");

            return 0;
        }
    }
}