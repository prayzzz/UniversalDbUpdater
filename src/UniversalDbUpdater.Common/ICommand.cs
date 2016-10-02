using System.Collections.Generic;

namespace UniversalDbUpdater.Common
{
    public interface ICommand
    {
        DatabaseType DatabaseType { get; }

        string[] CommandName { get; }

        int Execute(IEnumerable<string> arguments, Settings settings);
    }
}