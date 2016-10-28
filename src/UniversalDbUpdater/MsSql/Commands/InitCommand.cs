using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using UniversalDbUpdater.Common;
using static UniversalDbUpdater.MsSql.MsSqlDatabase;

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

            using (var connection = new SqlConnection(GetConnectionString(settings)))
            {
                connection.Open();

                if (IsTableAvailable(connection, settings))
                {
                    _console.WriteLine($"Table '{GetTableName(settings.Schema, settings.Table)}' already exists");
                    return 0;
                }

                var script = ResourceHelper.Current.GetEmbeddedFile(GetType().GetTypeInfo().Assembly, "UniversalDbUpdater.MsSql.Resources.DbScriptsTable.sql");
                script = script.Replace("##SCRIPTTABLE##", GetTableName(settings.Schema, settings.Table));

                if (string.IsNullOrEmpty(script))
                {
                    throw new FileNotFoundException("UniversalDbUpdater.MsSql.Resources.DbScriptsTable.sql");
                }

                using (var command = new SqlCommand(script, connection))
                {
                    _console.WriteLine($"Creating table '{GetTableName(settings.Schema, settings.Table)}'");
                    command.ExecuteNonQuery();
                }
            }

            return 0;
        }

        public static bool IsTableAvailable(SqlConnection connection, Settings settings)
        {
            var schema = settings.Schema.Replace("[", "").Replace("]", "");
            var table = settings.Table.Replace("[", "").Replace("]", "");

            using (var command = new SqlCommand($"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{schema}' AND TABLE_NAME = '{table}'", connection))
            {
                return command.ExecuteScalar() != null;
            }
        }
    }
}


