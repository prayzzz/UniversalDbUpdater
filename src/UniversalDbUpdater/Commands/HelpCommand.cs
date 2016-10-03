using System;
using System.Collections.Generic;
using System.Reflection;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.Commands
{
    public class HelpCommand : ICommand
    {
        public CommandType CommandType => CommandType.Common;

        public string[] CommandName => new[] { "h", "help" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            var help = ResourceHelper.Current.GetEmbeddedFile(Assembly.GetEntryAssembly(), "UniversalDbUpdater.Resources.help.txt");
            Console.WriteLine(help);

            return 1;
        }
    }
}