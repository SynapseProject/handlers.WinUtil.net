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
    public class HandlerParameters
    {
        [XmlElement]
        public String ParamValue1 { get; set; }
        [XmlElement]
        public bool ParamValue2 { get; set; }
        [XmlArrayItem(ElementName = "ParamValue3")]
        public List<String> ParamValues3 { get; set; }
    }
}
