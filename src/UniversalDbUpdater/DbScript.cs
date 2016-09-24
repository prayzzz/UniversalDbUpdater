using System;

namespace UniversalDbUpdater
{
    public class DbScript
    {
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string FileName => Date.ToString(Program.DateFormat) + "_" + Name.Replace(" ", "_") + ".sql";
        public int Id { get; set; }

        public string Name { get; set; }
    }
}