using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UniversalDbUpdater.Common;

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

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] Parameters => new[] { "-c", "--create" };

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

            var file = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MySql.Resources.ScriptTemplate.mysql");
            file = file.Replace("##DATE##", script.Date.ToString("s"));
            file = file.Replace("##NAME##", script.Name);
            file = file.Replace("##DESCRIPTION##", script.Description);

            File.WriteAllText(script.FileNameWithoutExtension + ".mysql", file);

            _console.WriteLine($"Script created: {script.FileNameWithoutExtension}");

            return 0;
        }

        public void HelpShort()
        {
            _console.WriteLine(" -c --create \t Creates a new script file");
        }
    }
}