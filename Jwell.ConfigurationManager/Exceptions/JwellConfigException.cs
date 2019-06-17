using System;

namespace Jwell.ConfigurationManager.Exceptions
{
    public class JwellConfigException : Exception
    {
        public JwellConfigException(string message) : base(message) { }
        public JwellConfigException(string message, Exception ex) : base(message, ex) { }
    }
}
