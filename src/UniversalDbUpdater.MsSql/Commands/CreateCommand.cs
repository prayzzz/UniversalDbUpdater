using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Commands
{
    public class CreateCommand : ICommand
    {
        private static CreateCommand _instance;

        private CreateCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new CreateCommand());

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] Command => new[] { "-c", "--create" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            Console.WriteLine("Creating new script...");
            Console.WriteLine();

            var script = new DbScript();
            script.Name = arguments.FirstOrDefault();

            while (string.IsNullOrEmpty(script.Name))
            {
                Console.WriteLine("Script name:");
                script.Name = Console.ReadLine();
                Console.WriteLine();
            }

            Console.WriteLine("Description:");
            script.Description = Console.ReadLine();
            script.Date = DateTime.Now;

            var file = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MsSql.Resources.ScriptTemplate.sql");
            file = file.Replace("##DATE##", script.Date.ToString("s"));
            file = file.Replace("##NAME##", script.Name);
            file = file.Replace("##DESCRIPTION##", script.Description);

            File.WriteAllText(script.FileNameWithoutExtension + ".sql", file);

            Console.WriteLine("\t Created script {0}", script.FileNameWithoutExtension);

            return 0;
        }

        public void HelpShort()
        {
            Console.WriteLine(" -c --create \t Creates a new script file");
        }
    }
}