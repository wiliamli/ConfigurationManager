using Jwell.ConfigurationManager.Core.Utils;

namespace Jwell.ConfigurationManager.Internals
{
    public interface IRepositoryChangeListener
    {
        /// <summary>
        /// Invoked when config repository changes. </summary>
        /// <param name="namespaceName"> the namespace of this repository change </param>
        /// <param name="newProperties"> the properties after change </param>
        void OnRepositoryChange(string namespaceName, Properties newProperties);
    }
}

