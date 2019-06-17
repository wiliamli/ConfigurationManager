using System;

namespace Jwell.ConfigurationManager.Logging
{
    public static class LogManager
    {
        /// <summary>
        /// Allows the <see cref="ILoggerProvider"/> to be set for this library. <see cref="Provider"/> can
        /// be set once, and must be set before any other library methods are used.
        /// </summary>
        public static ILoggerProvider Provider
        {
            internal get
            {
                _providerRetrieved = true;
                return _provider;
            }
            set
            {
                if (_providerRetrieved)
                    throw new InvalidOperationException("The logging provider must be set before any JwellConfigurationCenter methods are called.");

                _provider = value;
            }
        }

        public static ILogger CreateLogger(string name) => Provider.CreateLogger(name);
        public static ILogger CreateLogger(Type type) => Provider.CreateLogger(type.FullName);
        public static ILogger CreateLogger<T>() => Provider.CreateLogger(typeof(T).FullName);

        static ILoggerProvider _provider = new NoOpLoggerProvider();
        static bool _providerRetrieved;
    }
}
