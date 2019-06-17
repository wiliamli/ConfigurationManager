using Jwell.ConfigurationManager.Core;
using System.Threading.Tasks;

namespace Jwell.ConfigurationManager.Internals
{
    public interface IConfigManager
    {
        /// <summary>
        /// Get the config instance for the namespace specified. </summary>
        /// <param name="namespaceName"> the namespace </param>
        /// <returns> the config instance for the namespace </returns>
        Task<IConfig> GetConfig(string namespaceName);
    }
}

