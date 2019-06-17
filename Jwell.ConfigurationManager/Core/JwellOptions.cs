using Jwell.ConfigurationManager.Core;
using Jwell.ConfigurationManager.Enums;
using Jwell.ConfigurationManager.Foundation;
using Jwell.ConfigurationManager.Logging;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Configuration;

namespace Jwell.ConfigurationManager.Core
{
    public class JwellOptions : IJwellOptions
    {
        private static readonly string DefaultAuthorization = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:"));

        private string _authorization;

        public string AppId { get; set; }

        public string DataCenter { get; set; }

        public string Cluster { get; set; }

        public Env Env { get; set; }

        public string SubEnv { get; set; }

        public string LocalIp { get; set; } = NetworkInterfaceManager.HostIp;

        public string MetaServer { get; set; } = ConfigConsts.DefaultMetaServerUrl;

        public int Timeout { get; set; } = 5000;

        public string Authorization
        {
            get
            {
                if (string.IsNullOrEmpty(_authorization))
                    _authorization = DefaultAuthorization;
                return _authorization;
            }
            set
            {
                _authorization = value;
            }
        }

        public int RefreshInterval { get; set; } = 5 * 60 * 1000;

        public string LocalCacheDir { get; set; }

        public void initLocalCacheDir()
        {
            if (string.IsNullOrEmpty(LocalCacheDir))
                LocalCacheDir = Path.Combine(ConfigConsts.DefaultLocalCacheDir, AppId);
        }
        public void InitCluster()
        {
            //LPT and DEV will be treated as a cluster(lower case)
            if (string.IsNullOrWhiteSpace(Cluster) && (Env.Dev == Env || Env.Lpt == Env))
                Cluster = Env.ToString().ToLower();

            //Use data center as cluster
            if (string.IsNullOrWhiteSpace(Cluster))
                Cluster = DataCenter;

            //Use sub env as cluster
            if (string.IsNullOrWhiteSpace(Cluster))
                Cluster = SubEnv;

            //Use default cluster
            if (string.IsNullOrWhiteSpace(Cluster))
                Cluster = ConfigConsts.ClusterNameDefault;
        }
    }
}
