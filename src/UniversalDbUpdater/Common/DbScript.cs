using System;

namespace UniversalDbUpdater.Common
{
    public class DbScript
    {
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string FileNameWithoutExtension => Date.ToString(Constants.DateFormat) + "_" + Name.Replace(" ", "_");

        public int Id { get; set; }

        public string Name { get; set; }
    }
}