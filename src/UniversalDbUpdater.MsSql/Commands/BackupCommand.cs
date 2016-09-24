using System;
using System.Collections.Generic;
using System.IO;

namespace UniversalDbUpdater.MsSql.Commands
{
    /// <summary>
    ///     /b
    /// </summary>
    public class BackupCommand : ICommand
    {
        private static BackupCommand _instance;

        private BackupCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new BackupCommand());

        public int Execute(IEnumerable<string> arguments)
        {
            Console.WriteLine("Executing Db Backup");

            using (var sqlConnection = new SqlConnection(Program.ConnectionString))
            {
                sqlConnection.Open();

                var fileName = string.Format("{0}-{1}.bak", DateTime.Now.ToString(Program.DateFormat), Program.DbName);
                var backupDir = Path.GetFullPath(Program.BackupFolder);

                if (!Directory.Exists(backupDir))
                {
                    Console.WriteLine($"Creating directory {backupDir}");
                    Directory.CreateDirectory(backupDir);
                }

                var backFilePath = Path.Combine(backupDir, fileName);
                var query = string.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", Program.DbName, backFilePath);

                using (var command = new SqlCommand(query, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Backup created");
                Console.WriteLine("\t " + backFilePath);
            }

            return 0;
        }

        public void HelpShort()
        {
            Console.WriteLine("\t /b \t Creates a backup of the database");
        }
    }
}