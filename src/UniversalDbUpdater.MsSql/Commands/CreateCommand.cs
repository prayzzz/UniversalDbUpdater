using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniversalDbUpdater.Commands
{
    /// <summary>
    ///     /c
    /// </summary>
    public class CreateCommand : ICommand
    {
        private static CreateCommand _instance;

        private CreateCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new CreateCommand());

        public int Execute(IEnumerable<string> arguments)
        {
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

            var file = ResourceHelper.GetEmbeddedFile("ScriptTemplate.sql");
            file = file.Replace("##DATE##", script.Date.ToString("s"));
            file = file.Replace("##NAME##", script.Name);
            file = file.Replace("##DESCRIPTION##", script.Description);

            File.WriteAllText(script.FileName, file);

            Console.WriteLine("\t Created script {0}", script.FileName);

            return 0;
        }

        public void HelpShort()
        {
            Console.WriteLine("\t /c \t Creates a new script file");
        }
    }
}