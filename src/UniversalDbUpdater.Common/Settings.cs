namespace UniversalDbUpdater.Common
{
    public class Settings
    {
        public string BackupDirectory { get; set; }

        public DatabaseType Type { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }
    }
}