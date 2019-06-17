using Jwell.Configuration;
using Jwell.Configuration.Core;
using Jwell.ConfigurationManager;
using Jwell.ConfigurationManager.Core;
using Jwell.ConfigurationManager.Enums;
using Jwell.ConfigurationManager.Exceptions;
using Jwell.ConfigurationManager.Internals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class JwellWebHostBuilderExtension
    {
        public static IWebHostBuilder UseJwellConfigCenter(this IWebHostBuilder webBuilder)
        {
           return webBuilder.ConfigureServices((context, service) =>
            {
                var builder = new ConfigurationBuilder()
                     .SetBasePath(context.HostingEnvironment.ContentRootPath)
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddJsonFile("appCustomSettings.json", optional: true, reloadOnChange: true)
                     .AddJwellConfigurationCenter(context.Configuration.GetSection("ConfigurationServer"));
                var configuration = builder.Build();
                service.AddSingleton<IJwellConfiguration>(_ => configuration);
            });
        }
    }

    internal static class JwellConfigurationExtensions
    {
        internal static IJwellConfigurationBuilder AddJwellConfigurationCenter(this IConfigurationBuilder builder, IConfiguration jwellConfiguration) =>
            builder.AddJwellConfigurationCenter(jwellConfiguration.Get<JwellConfigurationOptions>());

        internal static IJwellConfigurationBuilder AddJwellConfigurationCenter(this IConfigurationBuilder builder, IJwellConfigurationOptions options)
        {
            if (null == options)
                throw new JwellConfigException($"appsettings配置文件不存在或者未在appSetting下配置【ConfigurationServer】节点.");
            if (options is JwellConfigurationOptions jo)
            {
                jo.InitCluster();
                jo.initLocalCacheDir();
            }

            var repositoryFactory = new ConfigRepositoryFactory(options);

            JwellConfigurationManager.SetJwellOptions(repositoryFactory);

            var jwellBuilder = new JwellConfigurationBuilder(builder, repositoryFactory);
            foreach (var @namespace in options.GetNamespace())
            {
                jwellBuilder.AddNamespace(@namespace);
            }
            return jwellBuilder;
        }
    }
}

namespace Jwell.Configuration.Core
{
    internal static class JwellConfigurationBuilderExtensions
    {
        /// <summary>添加其他namespace</summary>
        internal static IJwellConfigurationBuilder AddNamespace(this IJwellConfigurationBuilder builder, string @namespace) =>
            builder.AddNamespace(@namespace, null);

        /// <summary>添加其他namespace。如果sectionKey为null则添加到root中，可以直接读取，否则使用Configuration.GetSection(sectionKey)读取</summary>
        internal static IJwellConfigurationBuilder AddNamespace(this IJwellConfigurationBuilder builder, string @namespace, string sectionKey)
        {
            builder.Add(new JwellConfigurationProvider(sectionKey,@namespace, builder.ConfigRepositoryFactory.GetConfigRepository(@namespace ?? throw new ArgumentNullException(nameof(@namespace)))));

            return builder;
        }
    }
}