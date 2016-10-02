using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Commands
{
    public class BackupCommand : ICommand
    {
        private readonly IConsoleFacade _console;
        private readonly IDateTimeFacade _dateTime;

        public BackupCommand(IConsoleFacade console, IDateTimeFacade dateTime)
        {
            _console = console;
            _dateTime = dateTime;
        }

        public DatabaseType DatabaseType => DatabaseType.MsSql;

        public string[] CommandName => new[] { "b", "backup" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Creating backup...");
            _console.WriteLine();

            using (var sqlConnection = new SqlConnection(Database.GetConnectionString(settings)))
            {
                sqlConnection.Open();

                var fileName = string.Format("{0}-{1}.bak", _dateTime.Now.ToString(Constants.DateFormat), settings.Database);
                var backupDir = Path.GetFullPath(settings.BackupDirectory);

                if (!Directory.Exists(backupDir))
                {
                    _console.WriteLine($"Creating directory {backupDir}");
                    Directory.CreateDirectory(backupDir);
                }

                var backFilePath = Path.Combine(backupDir, fileName);
                var query = string.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", settings.Database, backFilePath);

                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }

                _console.WriteLine();
                _console.WriteLine("Backup created");
                _console.WriteLine("\t " + backFilePath);
            }

            return 0;
        }

        public void HelpShort()
        {
            _console.WriteLine(" b backup \t Creates a backup of the database");
        }
    }
}