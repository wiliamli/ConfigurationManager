using Jwell.Configuration.Core;
using Jwell.ConfigurationManager.Core;
using Jwell.ConfigurationManager.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Jwell.Configuration
{
    public class JwellConfiguration : ConfigurationRoot, IJwellConfiguration
    {
        public JwellConfiguration(IList<IConfigurationProvider> providers) : base(providers)
        { }

        public string GetAppSettingConfig(string key)
        {
            foreach (var item in this.Providers)
            {
                if (item is JsonConfigurationProvider jsonProvider)
                    if (Path.GetFileName(jsonProvider.Source.Path).Equals("appsettings.json", StringComparison.Ordinal))
                        if (item.TryGet(key, out string result))
                        {
                            return result.Trim();
                        }
            }
            throw new JwellConfigException($"appsettings配置文件不存在或者Key【{ key }】不存在与appsettings配置文件中.");
        }

        public string GetCustomSettingConfig(string key)
        {
            foreach (var item in this.Providers)
            {
                if (item is JsonConfigurationProvider jsonProvider)
                    if (Path.GetFileName(jsonProvider.Source.Path).Equals("appCustomSettings.json", StringComparison.Ordinal))
                        if (item.TryGet(key, out string result))
                        {
                            return result.Trim();
                        }
            }
            throw new JwellConfigException($"appCustomSettings配置文件不存在或者Key【{ key }】不存在与appCustomSettings配置文件中.");
        }

        public string GetConfig(string key, string @namespace = ConfigConsts.NamespaceApplication)
        {
            foreach (var item in this.Providers)
            {
                if (item is JwellConfigurationProvider jwellProvider)
                    if (jwellProvider.NameSpace.Equals(@namespace))
                        if (item.TryGet(key, out string result))
                        {
                            return result.Trim();
                        }
            }
            throw new JwellConfigException($"配置中心不存在{@namespace}命名空间或者Key【{ key }】不存在与{@namespace}命名空间中.");
        }
    }
}
