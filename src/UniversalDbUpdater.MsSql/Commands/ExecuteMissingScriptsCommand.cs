using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniversalDbUpdater.MsSql.Commands
{
    /// <summary>
    ///     /e
    ///     Creates a backup before executing scripts if /b is supplied
    /// </summary>
    public class ExecuteMissingScriptsCommand : ICommand
    {
        private static ExecuteMissingScriptsCommand _instance;

        private ExecuteMissingScriptsCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new ExecuteMissingScriptsCommand());

        public int Execute(IEnumerable<string> arguments)
        {
            var list = DbScriptHelper.GetMissingScripts().ToList();

            if (!list.Any())
            {
                Console.WriteLine("\t No missing scripts");
                return 0;
            }

            var args = arguments.ToList();
            if (args.Contains("/b"))
            {
                BackupCommand.Current.Execute(args.Skip(1));
                Console.WriteLine();
            }

            Console.WriteLine("Executing missing scripts:");
            Console.WriteLine();

            using (var sqlConnection = new SqlConnection(Program.ConnectionString))
            {
                sqlConnection.Open();
                var server = new Server(new ServerConnection(sqlConnection));

                foreach (var scriptName in list)
                {
                    server.ConnectionContext.BeginTransaction();

                    Console.WriteLine("\t {0}", scriptName);
                    var scriptContent = File.ReadAllText(scriptName);

                    try
                    {
                        server.ConnectionContext.ExecuteNonQuery(scriptContent);
                    }
                    catch (Exception ex)
                    {
                        server.ConnectionContext.RollBackTransaction();

                        Program.PrintException(ex);

                        Console.WriteLine();
                        Console.WriteLine("Script execution stopped");

                        return 1;
                    }

                    server.ConnectionContext.CommitTransaction();
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