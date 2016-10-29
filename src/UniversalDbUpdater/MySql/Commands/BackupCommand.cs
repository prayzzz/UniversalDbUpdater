using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql.Commands
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

        public CommandType CommandType => CommandType.MySql;

        public string[] CommandName => new[] { "b", "backup" };

        public int Execute(IEnumerable<string> arguments, Settings settings)
        {
            _console.WriteLine("Creating backup...");
            _console.WriteLine();

            var fileName = string.Format("{0}-{1}.mysql", _dateTime.Now.ToString(Constants.DateFormat), settings.Database);
            var backupFile = Path.Combine(settings.BackupDirectory, fileName);

            if (!Directory.Exists(settings.BackupDirectory))
            {
                Directory.CreateDirectory(settings.BackupDirectory);
                _console.WriteLine($"Directory created: {settings.BackupDirectory}");
            }

            var mysqlDump = GetMySqlDump();
            var cmd = $"{settings.Database} --host={settings.Host} --port={settings.Port} --user={settings.User} --password={settings.Password} --compress --result-file={backupFile}";

            var p = Process.Start(mysqlDump, cmd);
            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                return p.ExitCode;
            }

            _console.WriteLine();
            _console.WriteLine($"Backup created: {backupFile}");

            return p.ExitCode;
        }

        private static string GetMySqlDump()
        {
            var mysqlDump = "mysqldump";


            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (IsMysqlDumpInPath("where"))
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
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (IsMysqlDumpInPath("which"))
                {
                    return mysqlDump;
                }
            }

            throw new FileNotFoundException("mysqldump not found");
        }

        private static bool IsMysqlDumpInPath(string where)
        {
            try
            {
                var p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = where;
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
    }
}