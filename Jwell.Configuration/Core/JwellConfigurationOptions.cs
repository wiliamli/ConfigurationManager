using Jwell.ConfigurationManager.Core;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Jwell.ConfigurationManager;

namespace Jwell.Configuration.Core
{
    internal class JwellConfigurationOptions : JwellOptions, IJwellConfigurationOptions
    {
        public string Namespace { get; set; }
        public IEnumerable<string> GetNamespace()
        {
            yield return ConfigConsts.NamespaceApplication;
            if (!string.IsNullOrEmpty(Namespace))
            {
                foreach (var @namespace in Namespace.Split(';'))
                {
                    var value = @namespace.Trim();
                    if ((!string.IsNullOrEmpty(value)) && (!value.Equals(ConfigConsts.NamespaceApplication, StringComparison.Ordinal)))
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}
