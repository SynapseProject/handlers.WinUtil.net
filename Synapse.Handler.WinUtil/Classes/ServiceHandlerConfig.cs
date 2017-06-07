using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

using Synapse.Core;

using YamlDotNet.Serialization;

using Synapse.Core.Utilities;

namespace Synapse.Handlers.WinUtil
{
    public class ServiceHandlerConfig
    {
        [XmlElement]
        public ServiceAction Action { get; set; } = ServiceAction.Query;
        [XmlElement]
        public int Timeout { get; set; } = 0;
        [XmlElement]
        public bool RunSequential { get; set; } = false;
    }

}
