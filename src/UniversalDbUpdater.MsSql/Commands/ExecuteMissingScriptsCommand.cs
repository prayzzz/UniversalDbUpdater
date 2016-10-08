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
        private static readonly Regex GoRegexPattern = new Regex("^GO", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly IConsoleFacade _console;

        public ExecuteMissingScriptsCommand(IConsoleFacade console)
        {
            _console = console;
        }

        public CommandType CommandType => CommandType.MsSql;

        public string[] CommandName => new[] { "e", "execute" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Executing missing scripts...");
            _console.WriteLine();

            if (!Database.IsDbScriptsTableAvailable(settings))
            {
                _console.WriteLine("Table 'Infrastructure.DbScripts' doesn't exist");
                return 1;
            }

            var scriptFilePaths = ShowMissingScriptsCommand.GetMissingScripts(settings).ToList();

            if (!scriptFilePaths.Any())
            {
                _console.WriteLine("No missing scripts");
                return 0;
            }

            using (var connection = new SqlConnection(Database.GetConnectionString(settings)))
            {
                connection.Open();

                foreach (var filePath in scriptFilePaths)
                {
                    var transaction = connection.BeginTransaction();

                    _console.WriteLine($" {Path.GetFileName(filePath)}");
                    var scriptContent = GoRegexPattern.Replace(File.ReadAllText(filePath), "--GO");

                    using (var command = new SqlCommand())
                    {
                        command.Transaction = transaction;
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
    }
}