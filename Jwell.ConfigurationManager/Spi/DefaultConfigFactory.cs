using System.Threading.Tasks;
using Jwell.ConfigurationManager.Core;
using Jwell.ConfigurationManager.Internals;

namespace Jwell.ConfigurationManager.Spi
{
    public class DefaultConfigFactory : IConfigFactory
    {
        private readonly ConfigRepositoryFactory _repositoryFactory;

        public DefaultConfigFactory(ConfigRepositoryFactory repositoryFactory) => _repositoryFactory = repositoryFactory;

        public async Task<IConfig> Create(string namespaceName)
        {
            var configRepository = _repositoryFactory.GetConfigRepository(namespaceName);

            var config = new DefaultConfig(namespaceName, configRepository);

            await config.Initialize().ConfigureAwait(false);

            return config;
        }
    }
}
