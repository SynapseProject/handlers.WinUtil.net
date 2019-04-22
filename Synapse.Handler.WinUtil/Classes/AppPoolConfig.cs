using System;
using System.Xml.Serialization;
using System.Collections.Generic;

using Synapse.Core.Utilities;

namespace Synapse.Handlers.WinUtil
{
    [Serializable]
    public class AppPoolResults
    {
        [XmlArrayItem( ElementName = "AppPool" )]
        public List<AppPoolConfig> AppPools { get; set; } = new List<AppPoolConfig>();

        public void Add(AppPoolConfig pool)
        {
            lock( AppPools )
            {
                AppPools.Add( pool );
            }
        }

        public string ToXml(bool indent)
        {
            return XmlHelpers.Serialize<AppPoolResults>( this, true, true, indent );
        }

        public string ToYaml()
        {
            return YamlHelpers.Serialize( this );
        }

        public string ToJson(bool indent)
        {
            return YamlHelpers.Serialize( this, true, false, indent );
        }
    }

    [Serializable]
    public class AppPoolConfig : IProcessState
    {
        public AppPoolConfig()
        {
            ServerName = "Unknown";
            State = "Unknown";
            AppPoolName = "Unknown";
        }

        [XmlElement]
        public string ServerName { get; set; }
        [XmlElement]
        public string AppPoolName { get; set; }
        [XmlElement]
        public string LogOnAs { get; set; }
        [XmlElement]
        public string State { get; set; }


        public string ToXml(bool indent)
        {
            return XmlHelpers.Serialize<AppPoolConfig>( this, true, true, indent );
        }
    }
}