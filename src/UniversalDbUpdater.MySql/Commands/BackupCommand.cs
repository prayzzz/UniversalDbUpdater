using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Commands
{
    public class BackupCommand : ICommand
    {
        private static BackupCommand _instance;

        private BackupCommand()
        {
        }

        public static ICommand Current => _instance ?? (_instance = new BackupCommand());

        public DatabaseType DatabaseType => DatabaseType.MySql;

        public string[] Command => new[] { "-b", "--backup" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            Console.WriteLine("Executing Db Backup");
            Console.WriteLine();

            var fileName = string.Format("{0}-{1}.mysql", DateTime.Now.ToString(Constants.DateFormat), settings.Database);
            var backupDir = Path.GetFullPath(settings.BackupDirectory);
            var backFilePath = Path.Combine(backupDir, fileName);

            if (!Directory.Exists(backupDir))
            {
                Console.WriteLine($"Creating directory {backupDir}");
                Directory.CreateDirectory(backupDir);
            }

            var mysqlDump = GetMySqlDump();
            var cmd = $"{settings.Database} --host={settings.Host} --port={settings.Port} --user={settings.User} --password={settings.Password} --compress --result-file={backFilePath}";

            var p = Process.Start(mysqlDump, cmd);
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                return p.ExitCode;
            }

            Console.WriteLine();
            Console.WriteLine("Backup created");
            Console.WriteLine("\t " + backFilePath);

            return p.ExitCode;
        }

        private static string GetMySqlDump()
        {
            var mysqlDump = "mysqldump";

            if (IsMysqlDumpInPath())
            {
                return mysqlDump;
            }

            mysqlDump = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles"), @"MySQL\MySQL Server 5.7\bin\mysqldump.exe");

            if (File.Exists(mysqlDump))
            {
                return mysqlDump;
            }

            mysqlDump = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), @"MySQL\MySQL Server 5.7\bin\mysqldump.exe");

            if (File.Exists(mysqlDump))
            {
                return mysqlDump;
            }

            throw new FileNotFoundException("mysqldump not found");
        }

        private static bool IsMysqlDumpInPath()
        {
            try
            {
                var p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = "where";
                p.StartInfo.Arguments = "mysqldump";
                p.Start();
                p.WaitForExit();
                return p.ExitCode == 0;
            }
            catch (Win32Exception)
            {
                throw new Exception("'where' command is not on path");
            }
        }

        public void HelpShort()
        {
            Console.WriteLine("\t /b \t Creates a backup of the database");
        }
    }
}