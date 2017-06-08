using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

using YamlDotNet.Serialization;

using Synapse.Core.Utilities;

namespace Synapse.Handlers.WinUtil
{
    public class ServiceHandlerParameters
    {
        [XmlArrayItem(ElementName = "Service")]
        public List<Service> Services { get; set; }
    }

    public class Service
    {
        [XmlElement]
        public String Name { get; set; }
        [XmlElement]
        public String Server { get; set; } = @"localhost";
        [XmlElement]
        public String DisplayName { get; set; }
        [XmlElement]
        public String Description { get; set; }
        [XmlElement]
        public String BinPath { get; set; }
        [XmlElement]
        public ServiceStartMode StartMode { get; set; } = ServiceStartMode.Manual;
        [XmlElement]
        public String StartName { get; set; }
        [XmlElement]
        public String StartPassword { get; set; }
        [XmlElement]
        public List<String> StartParameters { get; set; }
        [XmlElement]
        public WindowsServiceType Type { get; set; } = WindowsServiceType.OwnProcess;
        [XmlElement]
        public ErrorControlAction OnError { get; set; } = ErrorControlAction.UserIsNotNotified;
        [XmlElement]
        public bool InteractWithDesktop { get; set; } = false;
        [XmlElement]
        public String LoadOrderGroup { get; set; }
        [XmlArrayItem(ElementName = "LoadOrderGroupDependency")]
        public List<String> LoadOrderGroupDependencies { get; set; }
        [XmlArrayItem(ElementName = "ServiceDependency")]
        public List<String> ServiceDependencies { get; set; }
    }
}
