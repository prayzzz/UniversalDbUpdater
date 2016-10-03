using System.Collections.Generic;

namespace UniversalDbUpdater.Common
{
    public interface ICommand
    {
        CommandType CommandType { get; }

        string[] CommandName { get; }

        int Execute(IEnumerable<string> arguments, Settings settings);
    }
}