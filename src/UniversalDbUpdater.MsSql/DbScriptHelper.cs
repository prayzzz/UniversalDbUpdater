using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UniversalDbUpdater.MsSql
{
    public class DbScriptHelper
    {
        //public static IEnumerable<string> GetMissingScripts()
        //{
        //    var localScripts = Directory.GetFiles(".", "*.sql").Select(Path.GetFileName).ToList();
        //    var dbScripts = new List<DbScript>();

        //    using (var sqlConnection = new SqlConnection(Program.ConnectionString))
        //    {
        //        sqlConnection.Open();

        //        using (var command = new SqlCommand("SELECT * FROM Infrastructure.DbScripts", sqlConnection))
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var script = new DbScript();
        //                script.Date = reader.GetDateTime(1);
        //                script.Name = reader.GetString(2);

        //                dbScripts.Add(script);
        //            }
        //        }
        //    }

        //    if (!dbScripts.Any())
        //    {
        //        return localScripts;
        //    }

        //    var missingScripts = new List<string>();

        //    foreach (var localScriptName in localScripts)
        //    {
        //        if (dbScripts.All(x => x.FileName != localScriptName))
        //        {
        //            missingScripts.Add(localScriptName);
        //        }
        //    }

        //    return missingScripts;
        //}
    }
}