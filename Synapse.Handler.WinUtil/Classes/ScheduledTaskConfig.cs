using System;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Win32.TaskScheduler;
using System.Collections.Generic;

namespace Synapse.Handlers.WinUtil
{
    [Serializable]
    public class ScheduledTaskResults
    {
        [XmlArrayItem(ElementName = "Task")]
        public List<ScheduledTaskConfig> Tasks { get; set; } = new List<ScheduledTaskConfig>();

        public void Add(ScheduledTaskConfig service)
        {
            lock (Tasks)
            {
                Tasks.Add(service);
            }
        }

        public string ToXml(bool indent)
        {
            return XmlHelpers.Serialize<ScheduledTaskResults>(this, true, true, indent);
        }

        public string ToYaml()
        {
            return YamlHelpers.Serialize(this);
        }

        public string ToJson(bool indent)
        {
            return YamlHelpers.Serialize(this, true, false, indent);
        }
    }

    [Serializable]
	public class ScheduledTaskConfig : IProcessState
	{
		public ScheduledTaskConfig()
		{
			ServerName = "Unknown";
			Name = "Unknown";
			State = "Unknown";
		}

		[XmlElement]
		public string ServerName { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement]
		public string State { get; set; }

		public string ToXml(bool indent)
		{
			string xml = XmlHelpers.Serialize<ScheduledTaskConfig>( this, true, true, indent );
			return xml;
		}
	}
}