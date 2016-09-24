using System.Collections.Generic;

namespace UniversalDbUpdater.Common
{
    public interface ICommand
    {
        int Execute(IEnumerable<string> arguments, Settings settings);

        void HelpShort();
    }
}