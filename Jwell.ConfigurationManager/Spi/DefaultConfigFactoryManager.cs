using System.Collections.Concurrent;
using Jwell.ConfigurationManager.Internals;

namespace Jwell.ConfigurationManager.Spi
{
    public class DefaultConfigFactoryManager : IConfigFactoryManager
    {
        private readonly IConfigRegistry _registry;
        private readonly IConfigFactory _configFactory;

        public DefaultConfigFactoryManager(IConfigRegistry registry, ConfigRepositoryFactory repositoryFactory)
        {
            _registry = registry;
            _configFactory = new DefaultConfigFactory(repositoryFactory);
        }

        public IConfigFactory GetFactory(string namespaceName) =>
            _registry.GetFactory(namespaceName) ?? _configFactory;
    }
}

