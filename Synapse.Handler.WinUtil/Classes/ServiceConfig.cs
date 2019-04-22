using System;
using System.Xml.Serialization;
using System.Collections.Generic;

using Synapse.Core.Utilities;

namespace Synapse.Handlers.WinUtil
{
    [Serializable]
    public class ServiceResults
    {
        [XmlArrayItem( ElementName = "Service" )]
        public List<ServiceConfig> Services { get; set; } = new List<ServiceConfig>();

        public void Add(ServiceConfig service)
        {
            lock( Services )
            {
                Services.Add( service );
            }
        }

        public string ToXml(bool indent)
        {
            return XmlHelpers.Serialize<ServiceResults>( this, true, true, indent );
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
    public class ServiceConfig : IProcessState
    {
        public ServiceConfig()
        {
            ServerName = "Unknown";
            State = "Unknown";
            ServiceName = "Unknown";
        }

        [XmlElement]
        public string ServerName { get; set; }
        [XmlElement]
        public string DisplayName { get; set; }
        [XmlElement]
        public string ServiceName { get; set; }
        [XmlElement]
        public string ServiceType { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement]
        public string PathName { get; set; }
        [XmlElement]
        public string LogOnAs { get; set; }
        [XmlElement]
        public ServiceStartMode StartMode { get; set; }
        [XmlElement]
        public string State { get; set; }
        [XmlElement]
        public bool AcceptStop { get; set; }
        [XmlElement]
        public int ProcessId { get; set; }
        [XmlElement]
        public string ErrorControl { get; set; }

        [XmlElement]
        public bool AcceptPause { get; set; }
        [XmlElement]
        public string Caption { get; set; }
        [XmlElement]
        public int CheckPoint { get; set; }
        [XmlElement]
        public string CreationClassName { get; set; }
        [XmlElement]
        public bool DelayedAutoStart { get; set; }
        [XmlElement]
        public bool DesktopInteract { get; set; }
        [XmlElement]
        public int ExitCode { get; set; }
        [XmlElement]
        public string InstallDate { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public int ServiceSpecificExitCode { get; set; }
        [XmlElement]
        public bool Started { get; set; }
        [XmlElement]
        public string StartName { get; set; }
        [XmlElement]
        public string Status { get; set; }
        [XmlElement]
        public string SystemCreationClassName { get; set; }
        [XmlElement]
        public string SystemName { get; set; }
        [XmlElement]
        public int TagId { get; set; }
        [XmlElement]
        public int WaitHint { get; set; }


        public string ToXml(bool indent)
        {
            return XmlHelpers.Serialize<ServiceConfig>( this, true, true, indent );
        }
    }
}