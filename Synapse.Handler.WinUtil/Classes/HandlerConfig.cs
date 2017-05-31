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
    public class HandlerConfig
    {
        [XmlElement]
        public String ConfigValue1 { get; set; }
        [XmlElement]
        public int ConfigValue2 { get; set; }
        [XmlArrayItem(ElementName ="ConfigValue3")]
        public List<String> ConfigValues3 { get; set; }
    }

}
