using Jwell.ConfigurationManager.Core;
using Jwell.ConfigurationManager.Exceptions;
using Jwell.ConfigurationManager.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;

namespace Jwell.ConfigurationManager
{
    public static class JwellConfiguration
    {
        private static IConfigurationRoot _configuration;

        static JwellConfiguration()
        {
            string basePath = Directory.GetCurrentDirectory();
             //basePath = AppContext.BaseDirectory.Split(new string[] { "\\bin" }, StringSplitOptions.None)[0];
            
            var builder = new ConfigurationBuilder()
                            .SetBasePath(basePath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile("appCustomSettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
        }

        /// <summary>
        /// 获取应用配置
        /// </summary>
        /// <param name="key">配置的键</param>
        /// <returns></returns>
        public static string GetAppSettingConfig(string key)
        {
            foreach (var item in _configuration.Providers)
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

        /// <summary>
        /// 获取用户自定义配置
        /// </summary>
        /// <param name="key">配置的键</param>
        /// <returns></returns>
        public static string GetCustomSettingConfig(string key)
        {
            foreach (var item in _configuration.Providers)
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

        /// <summary>
        /// 获取远程配置
        /// </summary>
        /// <param name="key">配置的键</param>
        /// <param name="namespace">配置命名空间</param>
        /// <returns></returns>
        public static string GetConfig(string key, string @namespace = ConfigConsts.NamespaceApplication)
        {
            var config = GetConfigManager().GetConfig(@namespace).GetAwaiter().GetResult();
            var value = config.GetProperty(key, null);
            if (value == null)
                throw new JwellConfigException($"Key【{key}】不存在与当前配置中心.");
            return value;
        }

        private static IConfigManager GetConfigManager()
        {
            var configManager = JwellConfigurationManager.GetConfigManager();
            if (configManager == null)
            {
                var options = _configuration.GetSection("ConfigurationServer").Get<JwellOptions>();
                if (null == options)
                    throw new JwellConfigException($"appsettings配置文件不存在或者未在appSetting下配置【ConfigurationServer】节点.");
                
                if (options is JwellOptions jo)
                {
                    jo.InitCluster();
                    jo.initLocalCacheDir();
                }

                var repositoryFactory = new ConfigRepositoryFactory(options);
                JwellConfigurationManager.SetJwellOptions(repositoryFactory);
                configManager = JwellConfigurationManager.GetConfigManager();
            }
            return configManager;
        }

    }
}
