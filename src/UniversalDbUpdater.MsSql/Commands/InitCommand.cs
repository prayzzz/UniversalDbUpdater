using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Commands
{
    public class InitCommand : ICommand
    {
        private readonly IConsoleFacade _console;

        public InitCommand(IConsoleFacade console)
        {
            _console = console;
        }

        public CommandType CommandType => CommandType.MsSql;

        public string[] CommandName => new[] { "i", "init" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Initializing database...");
            _console.WriteLine();

            using (var connection = new SqlConnection(Database.GetConnectionString(settings)))
            {
                connection.Open();

                if (IsTableAvailable(connection))
                {
                    _console.WriteLine("Table 'Infrastructure.DbScripts' already exists");
                    return 0;
                }

                var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MsSql.Resources.DbScriptsTable.sql");

                if (string.IsNullOrEmpty(script))
                {
                    throw new FileNotFoundException("UniversalDbUpdater.MsSql.Resources.DbScriptsTable.sql");
                }

                using (var command = new SqlCommand(script, connection))
                {
                    _console.WriteLine("Creating table 'Infrastructure.DbScripts'");
                    command.ExecuteNonQuery();
                }
            }

            return 0;
        }

        public static bool IsTableAvailable(SqlConnection connection)
        {
            using (var command = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Infrastructure' AND TABLE_NAME = 'DbScripts'", connection))
            {
                return command.ExecuteScalar() != null;
            }
        }
    }
}


