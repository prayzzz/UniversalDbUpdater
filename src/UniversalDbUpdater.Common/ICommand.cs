﻿using System.Collections.Generic;

namespace UniversalDbUpdater.Common
{
    public interface ICommand
    {
        DatabaseType DatabaseType { get; }
        string[] Command { get; }

        int Execute(IEnumerable<string> arguments, Settings settings);

        void HelpShort();
    }
}