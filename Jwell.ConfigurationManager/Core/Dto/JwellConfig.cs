using System.Collections.Generic;

namespace Jwell.ConfigurationManager.Core.Dto
{
    public class JwellConfig
    {
        public JwellConfig()
        {
        }

        public JwellConfig(string appId, string cluster, string namespaceName, string releaseKey)
        {
            AppId = appId;
            Cluster = cluster;
            NamespaceName = namespaceName;
            ReleaseKey = releaseKey;
        }

        public string AppId { get; set; }

        public string Cluster { get; set; }

        public string NamespaceName { get; set; }

        public string ReleaseKey { get; set; }

        public IDictionary<string, string> Configurations { get; set; }

        public override string ToString()
        {
            return "JwellConfig{" + "appId='" + AppId + '\'' + ", cluster='" + Cluster + '\'' +
                ", namespaceName='" + NamespaceName + '\'' + ", configurations=" + Configurations +
                ", releaseKey='" + ReleaseKey + '\'' + '}';
        }
    }
}
