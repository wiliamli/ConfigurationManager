using System;
using System.Threading.Tasks;
using Jwell.ConfigurationManager.Core.Utils;

namespace Jwell.ConfigurationManager.Internals
{
    public interface IConfigRepository : IDisposable
    {
        /// <summary>
        /// Get the config from this repository. </summary>
        /// <returns> config </returns>
        Properties GetConfig();

        Task Initialize();

        /// <summary>
        /// Add change listener. </summary>
        /// <param name="listener"> the listener to observe the changes </param>
        void AddChangeListener(IRepositoryChangeListener listener);

        /// <summary>
        /// Remove change listener. </summary>
        /// <param name="listener"> the listener to remove </param>
        void RemoveChangeListener(IRepositoryChangeListener listener);
    }
}

