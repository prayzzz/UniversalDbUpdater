using System.IO;
using System.Reflection;

namespace UniversalDbUpdater.Common
{
    public class ResourceHelper
    {
        private static ResourceHelper _instance;

        private ResourceHelper()
        {
        }

        public static ResourceHelper Current => _instance ?? (_instance = new ResourceHelper());

        public string GetEmbeddedFile(Assembly assembly, string fileName)
        {
            var resourceName = fileName;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return string.Empty;
        }
    }
}