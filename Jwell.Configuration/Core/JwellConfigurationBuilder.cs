using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Jwell.ConfigurationManager.Internals;

namespace Jwell.Configuration.Core
{
    internal interface IJwellConfigurationBuilder : IConfigurationBuilder
    {
        ConfigRepositoryFactory ConfigRepositoryFactory { get; }

        new JwellConfiguration Build();
    }

    internal class JwellConfigurationBuilder : IJwellConfigurationBuilder
    {
        private readonly IConfigurationBuilder _builder;

        public ConfigRepositoryFactory ConfigRepositoryFactory { get; }

        public JwellConfigurationBuilder(IConfigurationBuilder builder, ConfigRepositoryFactory configRepositoryFactory)
        {
            _builder = builder;

            ConfigRepositoryFactory = configRepositoryFactory;
        }

        public IConfigurationBuilder Add(IConfigurationSource source) => _builder.Add(source);

        public IConfigurationRoot Build() => _builder.Build();

        JwellConfiguration IJwellConfigurationBuilder.Build()
        {
            var providers = new List<IConfigurationProvider>();
            foreach (var source in Sources)
            {
                var provider = source.Build(this);
                providers.Add(provider);
            }
            return new JwellConfiguration(providers);
        }

        public IDictionary<string, object> Properties => _builder.Properties;
        public IList<IConfigurationSource> Sources => _builder.Sources;
    }
}
