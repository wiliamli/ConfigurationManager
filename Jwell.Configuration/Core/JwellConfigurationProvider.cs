using Jwell.ConfigurationManager.Core.Utils;
using Jwell.ConfigurationManager.Internals;
using Jwell.ConfigurationManager.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jwell.Configuration.Core
{
    internal class JwellConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IConfigurationSource
    {
        private readonly string _sectionKey;
        private readonly IConfigRepository _configRepository;
        private readonly Task _initializeTask;
        public string NameSpace { get; private set; }

        internal JwellConfigurationProvider(string sectionKey, string @namespace, IConfigRepository configRepository)
        {
            _sectionKey = sectionKey;
            _configRepository = configRepository;
            _initializeTask = _configRepository.Initialize();
            this.NameSpace = @namespace;
        }

        public override void Load()
        {
            _initializeTask.GetAwaiter().GetResult();

            _configRepository.AddChangeListener(this);

            SetData(_configRepository.GetConfig());
        }

        private void SetData(Properties properties)
        {
            Data = string.IsNullOrEmpty(_sectionKey) || properties.Source == null ? properties.Source : new Dictionary<string, string>(properties.Source.ToDictionary(kv => $"{_sectionKey}{ConfigurationPath.KeyDelimiter}{kv.Key}", kv => kv.Value), StringComparer.OrdinalIgnoreCase);
        }

        public void OnRepositoryChange(string namespaceName, Properties newProperties)
        {
            SetData(newProperties);

            OnReload();
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
    }
}
