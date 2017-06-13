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
    public class IISAppPoolHandlerParameters
    {
        [XmlArrayItem(ElementName = "IISAppPool")]
        public List<IISAppPool> IISAppPools { get; set; }
    }

    public class IISAppPool
    {
        [XmlElement]
        public String Name { get; set; }
        [XmlElement]
        public String Server { get; set; } = @"localhost";
        [XmlElement]
        public String ManagedRuntimeVersion { get; set; } = "v4.0";
        [XmlElement]
        public AppPoolIdentityType IdentityType { get; set; } = AppPoolIdentityType.ApplicationPoolIdentity;
        [XmlElement]
        public String Domain { get; set; }
        [XmlElement]
        public String UserName { get; set; }
        [XmlElement]
        public String Password { get; set; }
    }
}
