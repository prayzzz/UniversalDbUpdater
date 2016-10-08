namespace UniversalDbUpdater.Common
{
    public class ConsoleFacade : IConsoleFacade
    {
        public void WriteLine()
        {
            System.Console.WriteLine();
        }

        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }
    }

    public interface IConsoleFacade
    {
        void WriteLine();

        void WriteLine(string value);

        string ReadLine();
    }
}