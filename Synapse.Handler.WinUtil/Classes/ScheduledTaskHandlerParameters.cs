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
    public class ScheduledTaskHandlerParameters
    {
        [XmlArrayItem(ElementName = "Task")]
        public List<ScheduledTask> Tasks { get; set; }
    }

    public class ScheduledTask
    {
        [XmlElement]
        public String Name { get; set; }
        [XmlElement]
        public String Server { get; set; } = @"localhost";
    }
}
