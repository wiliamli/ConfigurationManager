using Jwell.ConfigurationManager.Core;
using Jwell.ConfigurationManager.Internals;
using Jwell.ConfigurationManager.Spi;
using System.Threading;
using System.Threading.Tasks;

namespace Jwell.ConfigurationManager
{
    /// <summary>
    /// Entry point for client config use
    /// </summary>
    public class JwellConfigurationManager
    {
        private static IConfigManager _manager;

        public static void SetJwellOptions(ConfigRepositoryFactory factory) =>
            Interlocked.CompareExchange(ref _manager, new DefaultConfigManager(new DefaultConfigFactoryManager(new DefaultConfigRegistry(), factory)), null);

        public static IConfigManager GetConfigManager()
        {
            return _manager;
        }

        /// <summary>
        /// Get Application's config instance. </summary>
        /// <returns> config instance </returns>
        public Task<IConfig> GetAppConfig() => GetConfig(ConfigConsts.NamespaceApplication);

        /// <summary>
        /// Get the config instance for the namespace. </summary>
        /// <param name="namespaceName"> the namespace of the config </param>
        /// <returns> config instance </returns>
        public Task<IConfig> GetConfig(string namespaceName) => _manager.GetConfig(namespaceName);
    }
}

