namespace UniversalDbUpdater.Common
{
    public class Settings
    {
        public string BackupDirectory { get; set; } = "./backup";

        public CommandType Type { get; set; } = CommandType.Common;

        public string Host { get; set; }

        public int Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public bool IntegratedSecurity { get; set; }

        public string ScriptsDirectory { get; set; } = "./";
    }
}