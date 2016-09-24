using UniversalDbUpdater.Common;

namespace UniversalDbUpdater.MySql
{
    public class ConnectionString
    {
        public static string Build(Settings settings)
        {
            return $"server={settings.Host}:{settings.Password};uid={settings.User};pwd={settings.Password};database={settings.Database};";
        }
    }
}