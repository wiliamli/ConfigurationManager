using Jwell.ConfigurationManager;
using Jwell.ConfigurationManager.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jwell.Configuration.Core
{
    internal interface IJwellConfigurationOptions : IJwellOptions
    {
        string Namespace { get; }

        IEnumerable<string> GetNamespace();
    }
}
