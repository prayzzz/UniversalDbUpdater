using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MsSql.Commands
{
    public class BackupCommand : ICommand
    {
        private static BackupCommand _instance;

        private BackupCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new BackupCommand());

        public DatabaseType DatabaseType => DatabaseType.MsSql;

        public string[] Command => new[] { "-b", "--backup" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            Console.WriteLine("Creating backup...");
            Console.WriteLine();

            using (var sqlConnection = new SqlConnection(Database.GetConnectionString(settings)))
            {
                sqlConnection.Open();

                var fileName = string.Format("{0}-{1}.bak", DateTime.Now.ToString(Constants.DateFormat), settings.Database);
                var backupDir = Path.GetFullPath(settings.BackupDirectory);

                if (!Directory.Exists(backupDir))
                {
                    Console.WriteLine($"Creating directory {backupDir}");
                    Directory.CreateDirectory(backupDir);
                }

                var backFilePath = Path.Combine(backupDir, fileName);
                var query = string.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", settings.Database, backFilePath);

                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine();
                Console.WriteLine("Backup created");
                Console.WriteLine("\t " + backFilePath);
            }

            return 0;
        }

        public void HelpShort()
        {
            Console.WriteLine(" -b --backup \t Creates a backup of the database");
        }
    }
}