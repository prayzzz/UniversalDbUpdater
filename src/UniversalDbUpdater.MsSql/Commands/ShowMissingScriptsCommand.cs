using System;
using System.Collections.Generic;

namespace UniversalDbUpdater.Commands
{
    public class ShowMissingScriptsCommand : ICommand
    {
        private static ShowMissingScriptsCommand _instance;

        private ShowMissingScriptsCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new ShowMissingScriptsCommand());

        /// <summary>
        ///     /m
        /// </summary>
        public int Execute(IEnumerable<string> arguments)
        {
            var missingScripts = DbScriptHelper.GetMissingScripts().ToList();

            if (!missingScripts.Any())
            {
                Console.WriteLine("\t No missing scripts");
                return 0;
            }


            Console.WriteLine("\t {0} missing scripts", missingScripts.Count);
            Console.WriteLine();

            foreach (var missingScript in missingScripts)
            {
                Console.WriteLine("\t {0}", missingScript);
            }

            return 0;
        }

        public void HelpShort()
        {
            Console.WriteLine("\t /m \t Shows scripts missing in database");
        }
    }
}