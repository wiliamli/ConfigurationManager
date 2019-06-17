using Jwell.ConfigurationManager.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jwell.Configuration
{
    /// <summary>
    /// 配置获取接口
    /// </summary>
    public interface IJwellConfiguration : IConfiguration
    {
        /// <summary>
        /// 获取应用配置
        /// </summary>
        /// <param name="key">配置的键</param>
        /// <returns></returns>
        string GetAppSettingConfig(string key);

        /// <summary>
        /// 获取用户自定义配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetCustomSettingConfig(string key);

        /// <summary>
        /// 获取远程配置
        /// </summary>
        /// <param name="key">配置的键</param>
        /// <param name="namespace">配置的命名空间</param>
        /// <returns></returns>

        string GetConfig(string key, string @namespace = ConfigConsts.NamespaceApplication);
    }
}
