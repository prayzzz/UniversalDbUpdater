namespace UniversalDbUpdater.Common
{
    public class DbScript
    {
        private string _name;

        //public string FileNameWithoutExtension => Date.ToString(Constants.DateFormat) + "_" + Name.Replace(" ", "_");

        public int Id { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value.Replace(" ", "_");
            }
        }
    }
}