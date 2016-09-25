using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Commands
{
    public class ExecuteMissingScriptsCommand : ICommand
    {
        private static readonly Regex GoRegexPattern = new Regex("^GO$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static ExecuteMissingScriptsCommand _instance;

        private ExecuteMissingScriptsCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new ExecuteMissingScriptsCommand());

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] Command => new[] { "-e", "--execute" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            Console.WriteLine("Executing missing scripts...");
            Console.WriteLine();

            if (!Database.IsDbScriptsTableAvailable(settings))
            {
                return 1;
            }

            var list = ShowMissingScriptsCommand.GetMissingScripts(settings).ToList();

            if (!list.Any())
            {
                Console.WriteLine("No missing scripts");
                return 0;
            }

            using (var connection = new SqlConnection(Database.GetConnectionString(settings)))
            {
                connection.Open();

                foreach (var scriptName in list)
                {
                    var transaction = connection.BeginTransaction();

                    Console.WriteLine("\t {0}", scriptName);
                    var scriptContent = GoRegexPattern.Replace(File.ReadAllText(scriptName), "--GO");

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = scriptContent;

                        try
                        {
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }

            return 0;
        }

        public void HelpShort()
        {
            Console.WriteLine("\t /e \t Executes missing scripts");
        }
    }
}