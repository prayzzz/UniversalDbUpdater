﻿using System.Collections.Generic;
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

        public CommandType CommandType => CommandType.MsSql;

        public string[] CommandName => new[] { "b", "backup" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Creating backup...");
            _console.WriteLine();

            using (var sqlConnection = new SqlConnection(MsSqlDatabase.GetConnectionString(settings)))
            {
                sqlConnection.Open();

                var fileName = string.Format("{0}-{1}.bak", _dateTime.Now.ToString(Constants.DateFormat), settings.Database);
                var backupDir = Path.GetFullPath(settings.BackupDirectory);

                if (!Directory.Exists(backupDir))
                {
                    _console.WriteLine($"Creating directory {backupDir}");
                    Directory.CreateDirectory(backupDir);
                }

                var backupFile = Path.Combine(backupDir, fileName);
                var query = string.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", settings.Database, backupFile);

                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }

                _console.WriteLine();
                _console.WriteLine($"Backup created: {backupFile}");
            }

            return 0;
        }
    }
}